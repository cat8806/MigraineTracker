namespace MigraineTracker.Services;

public static partial class SleepDataService
{
    public static partial Task<SleepData?> GetLatestSleepAsync()
    {
        return Task.FromResult<SleepData?>(null);
    }
}
