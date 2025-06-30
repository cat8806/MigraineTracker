using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel.DataTransfer;
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
        await Share.RequestAsync(new ShareTextRequest
        {
            Text = markdown,
            Title = "Migraine Diary"
        });
    }
}
