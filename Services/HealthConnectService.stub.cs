using System.Threading.Tasks;
using MigraineTracker.Models;

namespace MigraineTracker.Services;

public static partial class HealthConnectService
{
    /// <summary>
    /// Fallback implementation when Android Health Connect is not available.
    /// </summary>
    public static partial Task<SleepEntry?> GetLastSleepEntryAsync()
        => Task.FromResult<SleepEntry?>(null);
}
