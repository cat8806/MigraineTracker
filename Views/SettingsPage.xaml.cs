using System;
using Microsoft.Maui.Controls;
using MigraineTracker.Services;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;

namespace MigraineTracker.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void OnExportClicked(object sender, EventArgs e)
    {
        var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Permission Required", "Storage permission was denied.", "OK");
            return;
        }

        string? directory = await FolderPicker.PickFolderAsync();
        if (string.IsNullOrEmpty(directory))
            return;

        string path = await BackupService.ExportBackupAsync(directory);
        await DisplayAlert("Backup Exported", $"Backup saved to:\n{path}", "OK");
    }

    private async void OnImportClicked(object sender, EventArgs e)
    {
        var status = await Permissions.RequestAsync<Permissions.StorageRead>();
        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Permission Required", "Storage permission was denied.", "OK");
            return;
        }

        var file = await FilePicker.Default.PickAsync();
        if (file == null)
            return;

        await BackupService.ImportBackupAsync(file.FullPath);
        await DisplayAlert("Backup Imported", "The backup has been restored.", "OK");
    }
}
