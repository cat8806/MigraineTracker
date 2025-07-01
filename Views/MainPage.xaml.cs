using MigraineTracker.ViewModels;
namespace MigraineTracker.Views
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel vm;


        public MainPage()
        {
            InitializeComponent();
            vm = new MainPageViewModel();
            BindingContext = vm;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!vm.IsLoaded)
            {
                await vm.LoadDashboardAsync();
            }
        }
        private async void OnAddMigraineClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddMigrainePage));
        }
        private async void OnAddSupplementClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddSupplementPage));
        }
        private async void OnAddMealClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddMealPage));
        }
        private async void OnAddWaterClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddWaterPage));
        }
        private async void OnAddSleepClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddSleepPage));
        }
    }

}
