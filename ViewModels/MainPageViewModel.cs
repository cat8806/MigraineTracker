using System.ComponentModel;
using System.Runtime.CompilerServices;
using MigraineTracker.Data;
using MigraineTracker.Models;
using Microsoft.EntityFrameworkCore;
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
            _ = LoadLatestMigraineAsync();
        }

        public async Task LoadLatestMigraineAsync()
        {
            using (var db = new MigraineTrackerDbContext())
            {
                var latest = await db.Migraines
                    .OrderByDescending(m => m.Date)
                    .ThenByDescending(m => m.StartTime)
                    .FirstOrDefaultAsync();

                if (latest != null)
                {
                    LastMigraineEpisode = latest.Date.ToString("yyyy-MM-dd");
                    MigraineSeverity = $"Severity: {latest.Severity}";
                    if (!string.IsNullOrEmpty(latest.Triggers))
                    {
                        MigraineTriggers = $"Triggers: {latest.Triggers}";
                    }
                }
                else
                {
                    LastMigraineEpisode = "No migraine logged yet.";
                    MigraineSeverity = "";
                    MigraineTriggers = "";
                }
            }
        }
        public async Task LoadTodaySupplementsAsync()
        {
            using (var db = new MigraineTrackerDbContext())
            {
                var todaySupps = await db.Supplements
                    .Where(s => s.Date == DateTime.Today)
                    .GroupBy(s => new { s.Name, s.DosageUnit })
                    .Select(g => new
                    {
                        g.Key.Name,
                        g.Key.DosageUnit,
                        TotalDosage = g.Sum(x => x.DosageMg)
                    })
                    .OrderBy(r => r.Name)
                    .ToListAsync();

                var list = todaySupps
                    .Select(r => $"{r.Name} {r.TotalDosage} {r.DosageUnit}")
                    .ToList();

                SupplementList = string.Join("   • ", list);
            }
        }
        public async Task LoadTodayMealsAsync()
        {
            using var db = new MigraineTrackerDbContext();
            var list = await db.Meals
                .Where(m => m.Date == DateTime.Today && m.Time != null)   // ← filter out nulls
                .OrderBy(m => m.Time)
                .Select(m =>
                    $"{m.Time:hh:mm tt} {m.FoodItems}" +
                    (m.ContainsTrigger ? $" ({m.TriggerNotes})" : ""))
                .ToListAsync();

            TodayMeals = list.Any()
                ? string.Join("\n", list)
                : "No meals logged yet.";
        }

        public async Task LoadTodayWaterAsync()
        {
            using var db = new MigraineTrackerDbContext();
            var total = await db.WaterIntakes
                          .Where(w => w.Date == DateTime.Today)
                          .SumAsync(w => (int?)w.VolumeMl) ?? 0;

            const int dailyGoal = 2500;  // you can make this configurable
            WaterProgress = $"{total} mL / {dailyGoal} mL";
        }
        public async Task LoadLatestSleepAsync()
        {
            using var db = new MigraineTrackerDbContext();
            var latest = await db.Sleeps
                .OrderByDescending(s => s.Date)
                .ThenByDescending(s => s.SleepEnd)
                .FirstOrDefaultAsync();

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
