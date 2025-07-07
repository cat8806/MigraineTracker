using Android.Content;
using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Records;
using AndroidX.Health.Connect.Client.Request;

namespace MigraineTracker.Services;

public static partial class SleepDataService
{
    public static partial async Task<SleepData?> GetLatestSleepAsync()
    {
        var context = Android.App.Application.Context;
        var client = new HealthConnectClient(context);

        var request = new ReadRecordsRequest.Builder(typeof(SleepSessionRecord))
            .SetTimeRange(
                DateTimeOffset.Now.AddDays(-7),
                null,
                DateTimeOffset.Now,
                null)
            .Build();

        var response = await client.ReadRecordsAsync(request);
        var record = response.Records
            .OfType<SleepSessionRecord>()
            .OrderByDescending(r => r.EndTime)
            .FirstOrDefault();

        if (record == null)
            return null;

        return new SleepData(record.StartTime.UtcDateTime, record.EndTime.UtcDateTime);
    }
}
