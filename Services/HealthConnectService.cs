using System.Threading.Tasks;
using MigraineTracker.Models;

namespace MigraineTracker.Services;

public static partial class HealthConnectService
{
    /// <summary>
    /// Retrieves the most recent sleep session from Android Health Connect if available.
    /// Returns <c>null</c> on other platforms or when no session is found.
    /// </summary>
    public static partial Task<SleepEntry?> GetLastSleepEntryAsync();
}
