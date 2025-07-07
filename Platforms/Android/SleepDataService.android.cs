using Android.Content;
using Android.Health.Connect;
using Android.Health.Connect.Records;
using Android.Health.Connect.Request;

namespace MigraineTracker.Services;

public static partial class SleepDataService
{
    public static partial async Task<SleepData?> GetLatestSleepAsync()
    {
        var context = Android.App.Application.Context;
        var manager = new HealthConnectManager(context);

        var request = new ReadRecordsRequest.Builder(typeof(SleepSessionRecord))
            .SetTimeRange(
                DateTimeOffset.Now.AddDays(-7),
                null,
                DateTimeOffset.Now,
                null)
            .Build();

        var response = await manager.ReadRecordsAsync(request);
        var record = response.Records
            .OfType<SleepSessionRecord>()
            .OrderByDescending(r => r.EndTime)
            .FirstOrDefault();

        return record == null
            ? null
            : new SleepData(record.StartTime.UtcDateTime, record.EndTime.UtcDateTime);
    }
}
