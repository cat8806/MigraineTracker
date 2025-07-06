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

        try
        {
            using var stream = await file.OpenReadAsync();
            await BackupService.ImportBackupAsync(stream);
            await DisplayAlert("Backup Imported", "The backup has been restored.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Import Failed", $"An error occurred while importing the backup: {ex.Message}", "OK");
        }
    }
}
