using MigraineTracker.Views;

namespace MigraineTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AddMigrainePage), typeof(AddMigrainePage));
            Routing.RegisterRoute(nameof(AddSupplementPage), typeof(AddSupplementPage));
            Routing.RegisterRoute(nameof(AddMealPage), typeof(AddMealPage));
            Routing.RegisterRoute(nameof(AddDrinkPage), typeof(AddDrinkPage));
            Routing.RegisterRoute(nameof(AddSleepPage), typeof(AddSleepPage));
        }
    }
}
