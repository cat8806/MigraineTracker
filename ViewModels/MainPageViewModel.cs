using System.ComponentModel;
using System.Runtime.CompilerServices;
using MigraineTracker.Data;
using MigraineTracker.Models;
using System.Linq;

namespace MigraineTracker.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        // Sample properties for dashboard sections
        private string _lastMigraineEpisode = "";
        private string _migraineSeverity = "";
        private string _migraineTriggers = "";

        private string _supplementList = "";
        private string _todayMeals = "";
        private string _waterProgress = "";
        private string _sleepSummary = "";

        // Bindable Properties
        public string LastMigraineEpisode
        {
            get => _lastMigraineEpisode;
            set { _lastMigraineEpisode = value; OnPropertyChanged(); }
        }

        public string MigraineSeverity
        {
            get => _migraineSeverity;
            set { _migraineSeverity = value; OnPropertyChanged(); }
        }

        public string MigraineTriggers
        {
            get => _migraineTriggers;
            set { _migraineTriggers = value; OnPropertyChanged(); }
        }

        public string SupplementList
        {
            get => _supplementList;
            set { _supplementList = value; OnPropertyChanged(); }
        }

        public string TodayMeals
        {
            get => _todayMeals;
            set { _todayMeals = value; OnPropertyChanged(); }
        }

        public string WaterProgress
        {
            get => _waterProgress;
            set { _waterProgress = value; OnPropertyChanged(); }
        }

        public string SleepSummary
        {
            get => _sleepSummary;
            set { _sleepSummary = value; OnPropertyChanged(); }
        }

        public MainPageViewModel()
        {
            LoadLatestMigraine();
        }

        public void LoadLatestMigraine()
        {
            using (var db = new MigraineTrackerDbContext())
            {
                var latest = db.Migraines
                    .OrderByDescending(m => m.Date)
                    .ThenByDescending(m => m.StartTime)
                    .FirstOrDefault();

                if (latest != null)
                {
                    LastMigraineEpisode = latest.Date.ToString("yyyy-MM-dd");
                    MigraineSeverity = $"Severity: {latest.Severity}";
                    MigraineTriggers = $"Triggers: {latest.Triggers}";
                }
                else
                {
                    LastMigraineEpisode = "No entry yet";
                    MigraineSeverity = "—";
                    MigraineTriggers = "—";
                }
            }
        }
        public void LoadTodaySupplements()
        {
            using (var db = new MigraineTrackerDbContext())
            {
                var todaySupps = db.Supplements
                    .Where(s => s.Date == DateTime.Today)
                    .Select(s => $"{s.Name} {s.DosageMg}{s.DosageUnit}")
                    .ToList();

                SupplementList = string.Join("   • ", todaySupps);
            }
        }
        public void LoadTodayMeals()
        {
            using var db = new MigraineTrackerDbContext();
            var list = db.Meals
                .Where(m => m.Date == DateTime.Today && m.Time != null)   // ← filter out nulls
                .OrderBy(m => m.Time)
                .Select(m =>
                    $"{m.Time:hh:mm tt} {m.FoodItems}" +
                    (m.ContainsTrigger ? $" ({m.TriggerNotes})" : ""))
                .ToList();

            TodayMeals = list.Any()
                ? string.Join("\n", list)
                : "No meals logged yet.";
        }

        public void LoadTodayWater()
        {
            using var db = new MigraineTrackerDbContext();
            var total = db.WaterIntakes
                          .Where(w => w.Date == DateTime.Today)
                          .Sum(w => (int?)w.VolumeMl) ?? 0;

            const int dailyGoal = 2500;  // you can make this configurable
            WaterProgress = $"{total} mL / {dailyGoal} mL";
        }
        public void LoadLatestSleep()
        {
            using var db = new MigraineTrackerDbContext();
            var latest = db.Sleeps
                .OrderByDescending(s => s.Date)
                .ThenByDescending(s => s.SleepEnd)
                .FirstOrDefault();

            if (latest != null && latest.SleepStart.HasValue && latest.SleepEnd.HasValue)
            {
                var duration = (latest.SleepEnd.Value - latest.SleepStart.Value).TotalHours;
                SleepSummary =
                    $"{duration:F1} hr {latest.Quality}\n" +
                    $"{latest.SleepStart:hh:mm tt} – {latest.SleepEnd:hh:mm tt}\n" +
                    $"{latest.Notes}";
            }
            else
            {
                SleepSummary = "No sleep logged yet.";
            }
        }
        // Boilerplate for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    }
}
