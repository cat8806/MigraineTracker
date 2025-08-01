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

public class ReportGroup : ObservableCollection<ReportItem>
{
    public DateTime Date { get; }
    public string Header => Date.ToString("MMMM d yyyy");

    public ReportGroup(DateTime date, IEnumerable<ReportItem> items) : base(items)
    {
        Date = date;
    }
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
    public ObservableCollection<ReportGroup> ReportGroups { get; } = new();

    public async Task LoadDataAsync()
    {
        Items.Clear();
        var list = new List<ReportItem>();
        using var db = new MigraineTrackerDbContext();

        var sleeps = await db.Sleeps
            .Where(s => s.Date >= StartDate && s.Date <= EndDate)
            .ToListAsync();

        foreach (var s in sleeps)
        {
            var time = s.SleepStart ?? s.Date;
            list.Add(new ReportItem
            {
                Time = time,
                Icon = "\uD83D\uDE34", // 😴
                Text = $"Sleep {s.DurationHours:F1}h {s.Quality}"
            });
        }

        var migraines = await db.Migraines
            .Where(m => m.Date >= StartDate && m.Date <= EndDate)
            .ToListAsync();

        foreach (var m in migraines)
        {
            var time = m.StartTime ?? m.Date;

            var segments = new List<string>
            {
                $"Migraine Severity: {m.Severity}/10"               // now explicitly labeled
            };

            if (!string.IsNullOrWhiteSpace(m.Posture))
                segments.Add($"Posture: {m.Posture}");

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

        var meals = await db.Meals
            .Where(meal => meal.Date >= StartDate && meal.Date <= EndDate)
            .ToListAsync();

        foreach (var meal in meals)
        {
            var time = meal.Time ?? meal.Date;
            list.Add(new ReportItem
            {
                Time = time,
                Icon = "\uD83C\uDF7D", // 🍽
                Text = $"{meal.MealType}: {meal.FoodItems}"
            });
        }

        var supplements = await db.Supplements
            .Where(s => s.Date >= StartDate && s.Date <= EndDate)
            .ToListAsync();

        foreach (var s in supplements)
        {
            var time = s.TimeTaken ?? s.Date;
            list.Add(new ReportItem
            {
                Time = time,
                Icon = "\uD83D\uDC8A", // 💊
                Text = $"{s.Name} {s.DosageUnit} * {s.DosageMg}"
            });
        }

        var drinks = await db.Drinks
            .Where(w => w.Date >= StartDate && w.Date <= EndDate)
            .ToListAsync();

        foreach (var w in drinks)
        {
            var time = w.Time ?? w.Date;
            list.Add(new ReportItem
            {
                Time = time,
                Icon = "\uD83D\uDCA7", // 💧
                Text = $"{w.DrinkType} {w.VolumeMl} mL"
            });
        }

        foreach (var item in list.OrderBy(i => i.Time))
            Items.Add(item);

        ReportGroups.Clear();
        var grouped = Items
            .GroupBy(i => i.Time.Date)
            .OrderBy(g => g.Key);

        foreach (var grp in grouped)
        {
            var groupItems = grp.OrderBy(i => i.Time);
            ReportGroups.Add(new ReportGroup(grp.Key, groupItems));
        }
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
