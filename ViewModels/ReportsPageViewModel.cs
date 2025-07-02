using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using MigraineTracker.Data;
using MigraineTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace MigraineTracker.ViewModels;

public class ReportItem
{
    public DateTime Time { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}


public partial class ReportsPageViewModel : ObservableObject
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

    public ObservableCollection<ReportItem> Items { get; } = new();

    public async Task LoadDataAsync()
    {
        Items.Clear();
        var list = new List<ReportItem>();
        using var db = new MigraineTrackerDbContext();

        for (var date = StartDate.Date; date <= EndDate.Date; date = date.AddDays(1))
        {
            foreach (var s in await db.Sleeps.Where(s => s.Date == date).ToListAsync())
            {
                var time = s.SleepStart ?? date;
                list.Add(new ReportItem
                {
                    Time = time,
                    Icon = "\uD83D\uDE34", // 😴
                    Text = $"Sleep {s.DurationHours:F1}h {s.Quality}"
                });
            }

            foreach (var m in await db.Migraines.Where(m => m.Date == date).ToListAsync())
            {
                var time = m.StartTime ?? date;

                var segments = new List<string>
                {
                    $"Migraine Severity: {m.Severity}/10"               // now explicitly labeled
                };

                if (!string.IsNullOrWhiteSpace(m.Triggers))
                    segments.Add($"Triggers: {m.Triggers.Replace(";", ", ")}");

                if (!string.IsNullOrWhiteSpace(m.Notes))
                    segments.Add($"Note: {m.Notes}");

                list.Add(new ReportItem
                {
                    Time = time,
                    Icon = "\uD83D\uDCA5",   // 💥
                    Text = string.Join(" • ", segments)
                    // e.g. “Severity: 7/10 • Triggers: Stress, Cheese • Notes: Took medication”
                });
            }

            foreach (var meal in await db.Meals.Where(meal => meal.Date == date).ToListAsync())
            {
                var time = meal.Time ?? date;
                list.Add(new ReportItem
                {
                    Time = time,
                    Icon = "\uD83C\uDF7D", // 🍽
                    Text = $"{meal.MealType}: {meal.FoodItems}"
                });
            }

            foreach (var s in await db.Supplements.Where(s => s.Date == date).ToListAsync())
            {
                var time = s.TimeTaken ?? date;
                list.Add(new ReportItem
                {
                    Time = time,
                    Icon = "\uD83D\uDC8A", // 💊
                    Text = $"{s.Name} {s.DosageMg} {s.DosageUnit}"
                });
            }

            foreach (var w in await db.WaterIntakes.Where(w => w.Date == date).ToListAsync())
            {
                var time = w.Time ?? date;
                list.Add(new ReportItem
                {
                    Time = time,
                    Icon = "\uD83D\uDCA7", // 💧
                    Text = $"Water {w.VolumeMl} mL"
                });
            }
        }
        foreach (var item in list.OrderBy(i => i.Time))
            Items.Add(item);
    }

    public string BuildMarkdown()
    {
        var sb = new StringBuilder();
        for (var date = StartDate.Date; date <= EndDate.Date; date = date.AddDays(1))
        {
            sb.AppendLine($"# Diary for {date:yyyy-MM-dd}");
            foreach (var item in Items.Where(i => i.Time.Date == date).OrderBy(i => i.Time))
            {
                sb.AppendLine($"{item.Time:HH:mm} {item.Icon} {item.Text}");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
