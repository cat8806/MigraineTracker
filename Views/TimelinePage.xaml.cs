using Microsoft.Maui.Controls;
using MigraineTracker.ViewModels;

namespace MigraineTracker.Views;

public partial class TimelinePage : ContentPage
{
    private readonly TimelinePageViewModel vm;

    public TimelinePage()
    {
        InitializeComponent();
        vm = BindingContext as TimelinePageViewModel ?? new TimelinePageViewModel();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.LoadDataAsync();
    }
}
