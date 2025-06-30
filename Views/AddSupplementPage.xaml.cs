using MigraineTracker.ViewModels;

namespace MigraineTracker.Views
{
    public partial class AddSupplementPage : ContentPage
    {
        public AddSupplementPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as AddSupplementViewModel)?.LoadRecentBatches();
        }
    }
}
