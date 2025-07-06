using System;
using System.Threading.Tasks;

namespace MigraineTracker.Services;

public static partial class FolderPicker
{
    public static partial Task<string?> PickFolderAsync()
    {
        string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Task.FromResult<string?>(documents);
    }
}
