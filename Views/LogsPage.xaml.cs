using Microsoft.Maui.Controls;
using MigraineTracker.ViewModels;

namespace MigraineTracker.Views;

public partial class LogsPage : ContentPage
{
    private readonly LogsPageViewModel vm;

    public LogsPage()
    {
        InitializeComponent();
        vm = BindingContext as LogsPageViewModel ?? new LogsPageViewModel();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.LoadDataAsync();
    }
}
