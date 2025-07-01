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
    private DateTime _selectedDate = DateTime.Today;
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
            {
                LoadDataAsync();
            }
        }
    }

    public ObservableCollection<ReportItem> Items { get; } = new();

    public async Task LoadDataAsync()
    {
        Items.Clear();
        var list = new List<ReportItem>();
        var date = SelectedDate.Date;
        using var db = new MigraineTrackerDbContext();

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
            list.Add(new ReportItem
            {
                Time = time,
                Icon = "\uD83D\uDCA5", // 💥
                Text = $"Migraine {m.Severity}/10 {m.Triggers}"
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

        foreach (var item in list.OrderBy(i => i.Time))
            Items.Add(item);
    }

    public string BuildMarkdown()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# Diary for {SelectedDate:yyyy-MM-dd}");
        foreach (var item in Items.OrderBy(i => i.Time))
        {
            sb.AppendLine($"{item.Time:HH:mm} {item.Icon} {item.Text}");
        }
        return sb.ToString();
    }
}
