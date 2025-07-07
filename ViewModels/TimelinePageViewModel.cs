using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MigraineTracker.Data;
using MigraineTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace MigraineTracker.ViewModels
{
    public enum LogItemType
    {
        Migraine,
        Supplement,
        Meal,
        Water,
        Sleep
    }

    public class LogItem
    {
        public Guid Id { get; set; }
        public LogItemType ItemType { get; set; }
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string PrimaryText { get; set; } = string.Empty;
        public string SubText { get; set; } = string.Empty;
    }

    public class LogGroup : ObservableCollection<LogItem>
    {
        public DateTime Date { get; }
        public string Header => Date.ToString("yyyy-MM-dd");
        public LogGroup(DateTime date, IEnumerable<LogItem> items) : base(items)
        {
            Date = date;
        }
    }

    public partial class TimelinePageViewModel : ObservableObject
    {
        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    if (EndDate < _startDate)
                        EndDate = _startDate;
                    LoadDataAsync();
                }
            }
        }

        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    if (_endDate < StartDate)
                        StartDate = _endDate;
                    LoadDataAsync();
                }
            }
        }

        public ObservableCollection<LogGroup> LogGroups { get; } = new();

        public IRelayCommand<LogItem> DeleteCommand { get; }

        public TimelinePageViewModel()
        {
            DeleteCommand = new RelayCommand<LogItem>(DeleteItem);
        }

        public async Task LoadDataAsync()
        {
            LogGroups.Clear();
            var items = new List<LogItem>();
            using var db = new MigraineTrackerDbContext();

            foreach (var m in await db.Migraines
                .Where(m => m.Date >= StartDate && m.Date <= EndDate)
                .ToListAsync())
            {
                var time = m.StartTime ?? m.Date;
                items.Add(new LogItem
                {
                    Id = m.Id,
                    ItemType = LogItemType.Migraine,
                    Timestamp = time,
                    Icon = "\uD83D\uDCA5",
                    PrimaryText = $"Migraine  {new string('\u25CF', m.Severity)}   {m.Triggers}",
                    SubText = $"{time:hh:mm tt}"
                });
            }

            foreach (var s in await db.Supplements
                .Where(s => s.Date >= StartDate && s.Date <= EndDate)
                .ToListAsync())
            {
                var time = s.TimeTaken ?? s.Date;
                items.Add(new LogItem
                {
                    Id = s.Id,
                    ItemType = LogItemType.Supplement,
                    Timestamp = time,
                    Icon = "\uD83D\uDC8A",
                    PrimaryText = $"{s.Name} {s.DosageUnit} * {s.DosageMg}",
                    SubText = $"{time:hh:mm tt}"
                });
            }

            foreach (var m in await db.Meals
                .Where(m => m.Date >= StartDate && m.Date <= EndDate)
                .ToListAsync())
            {
                var time = m.Time ?? m.Date;
                items.Add(new LogItem
                {
                    Id = m.Id,
                    ItemType = LogItemType.Meal,
                    Timestamp = time,
                    Icon = "\uD83C\uDF7D",
                    PrimaryText = $"{m.MealType}: {m.FoodItems}",
                    SubText = $"{time:hh:mm tt}"
                });
            }

            foreach (var w in await db.WaterIntakes
                .Where(w => w.Date >= StartDate && w.Date <= EndDate)
                .ToListAsync())
            {
                var time = w.Time ?? w.Date;
                items.Add(new LogItem
                {
                    Id = w.Id,
                    ItemType = LogItemType.Water,
                    Timestamp = time,
                    Icon = "\uD83D\uDCA7",
                    PrimaryText = $"Water {w.VolumeMl} mL",
                    SubText = $"{time:hh:mm tt}"
                });
            }

            foreach (var s in await db.Sleeps
                .Where(s => s.Date >= StartDate && s.Date <= EndDate)
                .ToListAsync())
            {
                var time = s.SleepStart ?? s.Date;
                items.Add(new LogItem
                {
                    Id = s.Id,
                    ItemType = LogItemType.Sleep,
                    Timestamp = time,
                    Icon = "\uD83D\uDE34",
                    PrimaryText = $"Sleep {s.DurationHours:F1}h {s.Quality}",
                    SubText = s.SleepStart.HasValue && s.SleepEnd.HasValue
                        ? $"{s.SleepStart:hh:mm tt} – {s.SleepEnd:hh:mm tt}"
                        : string.Empty
                });
            }

            var grouped = items
                .OrderByDescending(i => i.Timestamp)
                .GroupBy(i => i.Timestamp.Date)
                .OrderByDescending(g => g.Key);

            foreach (var grp in grouped)
            {
                var groupItems = grp.OrderByDescending(i => i.Timestamp);
                LogGroups.Add(new LogGroup(grp.Key, groupItems));
            }
        }

        private void DeleteItem(LogItem item)
        {
            var group = LogGroups.FirstOrDefault(g => g.Contains(item));
            if (group != null)
            {
                group.Remove(item);
                if (group.Count == 0)
                    LogGroups.Remove(group);
            }

            using var db = new MigraineTrackerDbContext();
            switch (item.ItemType)
            {
                case LogItemType.Migraine:
                    db.Migraines.Remove(new MigraineEntry { Id = item.Id });
                    break;
                case LogItemType.Supplement:
                    db.Supplements.Remove(new SupplementEntry { Id = item.Id });
                    break;
                case LogItemType.Meal:
                    db.Meals.Remove(new MealEntry { Id = item.Id });
                    break;
                case LogItemType.Water:
                    db.WaterIntakes.Remove(new WaterIntakeEntry { Id = item.Id });
                    break;
                case LogItemType.Sleep:
                    db.Sleeps.Remove(new SleepEntry { Id = item.Id });
                    break;
            }
            db.SaveChanges();
        }
    }
}
