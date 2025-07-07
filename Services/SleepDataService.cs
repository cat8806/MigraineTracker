namespace MigraineTracker.Services;

public record SleepData(DateTime Start, DateTime End)
{
    public double DurationHours => (End - Start).TotalHours;
}

public static partial class SleepDataService
{
    public static partial Task<SleepData?> GetLatestSleepAsync();
}
