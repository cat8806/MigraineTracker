using MigraineTracker.ViewModels;

namespace MigraineTracker.Views
{
    public partial class AddSupplementPage : ContentPage
    {
        public AddSupplementPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is AddSupplementViewModel vm)
            {
                await vm.LoadRecentBatchesAsync();
                await vm.LoadSupplementNamesAsync();
            }
        }
    }
}
