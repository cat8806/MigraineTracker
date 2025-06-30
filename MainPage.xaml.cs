using MigraineTracker.ViewModels;
using MigraineTracker.Views;
namespace MigraineTracker
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private MainPageViewModel vm;


        public MainPage()
        {
            InitializeComponent();
            vm = new MainPageViewModel();
            BindingContext = vm;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.LoadLatestMigraine();  // Refresh when page comes back into view
            vm.LoadTodaySupplements();
            vm.LoadTodayMeals();
            vm.LoadTodayWater();
            vm.LoadLatestSleep();
        }
        private void OnCounterClicked(object sender, EventArgs e)
        {

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
