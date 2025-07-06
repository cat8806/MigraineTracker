using System;
using Microsoft.Maui.Controls;
using MigraineTracker.Services;
using Microsoft.Maui.Storage;

namespace MigraineTracker.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void OnExportClicked(object sender, EventArgs e)
    {
        string path = await BackupService.ExportBackupAsync();
        await DisplayAlert("Backup Exported", $"Backup saved to:\n{path}", "OK");
    }

    private async void OnImportClicked(object sender, EventArgs e)
    {
        var file = await FilePicker.Default.PickAsync();
        if (file == null)
            return;

        await BackupService.ImportBackupAsync(file.FullPath);
        await DisplayAlert("Backup Imported", "The backup has been restored.", "OK");
    }
}
