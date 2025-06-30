using MigraineTracker.Data;
namespace MigraineTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

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