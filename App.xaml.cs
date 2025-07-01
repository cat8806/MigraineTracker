using MigraineTracker.Data;
namespace MigraineTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Force the application to always use the light theme
            // regardless of the device's system setting.
            App.Current.UserAppTheme = AppTheme.Light;

            using (var db = new MigraineTrackerDbContext())
            {
                db.Database.EnsureCreated();
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}