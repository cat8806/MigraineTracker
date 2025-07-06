using Android.App;
using Android.Content;
using Microsoft.Maui.ApplicationModel;

namespace MigraineTracker.Services;

public static partial class FolderPicker
{
    static TaskCompletionSource<string?>? _tcs;
    const int RequestCode = 4242;

    public static partial Task<string?> PickFolderAsync()
    {
        var activity = Platform.CurrentActivity ?? throw new InvalidOperationException("No current Activity");
        _tcs = new TaskCompletionSource<string?>();

        var intent = new Intent(Intent.ActionOpenDocumentTree);
        intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantPersistableUriPermission);
        activity.StartActivityForResult(intent, RequestCode);

        return _tcs.Task;
    }

    internal static void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        if (requestCode != RequestCode)
            return;

        if (resultCode == Result.Ok && data?.Data != null)
        {
            var uri = data.Data;
            var flags = data.Flags & (ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
            Platform.CurrentActivity!.ContentResolver.TakePersistableUriPermission(uri!, flags);
            _tcs?.TrySetResult(uri!.ToString());
        }
        else
        {
            _tcs?.TrySetResult(null);
        }
    }
}
