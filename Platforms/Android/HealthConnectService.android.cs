#if ANDROID
using System;
using System.Linq;
using System.Threading.Tasks;
using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Records;
using AndroidX.Health.Connect.Client.Request;
using AndroidX.Health.Connect.Client.Time;
using Microsoft.Maui.ApplicationModel;
using MigraineTracker.Models;

namespace MigraineTracker.Services;

public static partial class HealthConnectService
{
    public static partial async Task<SleepEntry?> GetLastSleepEntryAsync()
    {
        var context = Platform.AppContext ?? Android.App.Application.Context;
        var client = new HealthConnectClient(context);

        var end = Instant.Now;
        var start = end.Minus(Duration.FromDays(7));

        var request = new ReadRecordsRequest.Builder(typeof(SleepSessionRecord))
            .SetTimeRangeFilter(TimeRangeFilter.Companion.Between(start, end))
            .Build();

        var response = await client.ReadRecordsAsync<SleepSessionRecord>(request);
        var latest = response.Records
            .OrderByDescending(r => r.EndTime)
            .FirstOrDefault();

        if (latest == null)
            return null;

        return new SleepEntry
        {
            Id = Guid.NewGuid(),
            Date = latest.StartTime.ToDateTimeOffset().Date,
            SleepStart = latest.StartTime.ToDateTimeOffset().DateTime,
            SleepEnd = latest.EndTime.ToDateTimeOffset().DateTime,
            Quality = "Fair",
            Notes = "Imported from Health Connect"
        };
    }
}
#endif
