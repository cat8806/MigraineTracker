using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using MigraineTracker.ViewModels;

namespace MigraineTracker.Views;

public partial class ReportsPage : ContentPage
{
    private readonly ReportsPageViewModel vm;

    public ReportsPage()
    {
        InitializeComponent();
        vm = BindingContext as ReportsPageViewModel ?? new ReportsPageViewModel();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.LoadDataAsync();
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        var markdown = vm.BuildMarkdown();
        var fileName = $"MigraineDiary_{vm.StartDate:yyyyMMdd}_{vm.EndDate:yyyyMMdd}.md";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        File.WriteAllText(filePath, markdown);

        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "Migraine Diary",
            File = new ShareFile(filePath)
        });
    }
}
