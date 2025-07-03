using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MigraineTracker.Data;
using MigraineTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace MigraineTracker.ViewModels
{
    public class SupplementBatchVM
    {
        public DateTime BatchDate { get; set; }
        public List<SupplementEntry> Items { get; set; }
        public string BatchDescription => string.Join(", ", Items.Select(i => $"{i.Name} {i.DosageMg} {i.DosageUnit}"));
    }

    public partial class AddSupplementViewModel : ObservableObject
    {
        public ObservableCollection<SupplementEntryDraft> SupplementDrafts { get; } = new();

        public ObservableCollection<SupplementBatchVM> RecentBatches { get; } = new();

        public ObservableCollection<string> SupplementNameSuggestions { get; } = new();

        public AddSupplementViewModel()
        {
            // Start with one row by default
            // Start with one row by default, using the current time
            SupplementDrafts.Add(new SupplementEntryDraft
            {
                TimeTaken = DateTime.Now.TimeOfDay
            });
        }

        [RelayCommand]
        public void AddSupplementRow()
        {
            // grab the time from the first row (if any)
            TimeSpan? defaultTime = SupplementDrafts.FirstOrDefault()?.TimeTaken;

            // create the new draft, copying only the time
            var draft = new SupplementEntryDraft
            {
                TimeTaken = defaultTime
            };

            SupplementDrafts.Add(draft);
        }

        [RelayCommand]
        public async Task SaveAllSupplementsAsync()
        {
            if (!SupplementDrafts.Any(d => !string.IsNullOrWhiteSpace(d.Name)))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter at least one supplement.", "OK");
                return;
            }

            var sessionId = Guid.NewGuid();
            using (var db = new MigraineTrackerDbContext())
            {
                foreach (var draft in SupplementDrafts)
                {
                    if (string.IsNullOrWhiteSpace(draft.Name)) continue;
                    var entry = new SupplementEntry
                    {
                        Id = Guid.NewGuid(),
                        Date = DateTime.Today,
                        Name = draft.Name,
                        DosageMg = draft.DosageMg,
                        DosageUnit = draft.DosageUnit,
                        TimeTaken = draft.TimeTaken.HasValue ? DateTime.Today + draft.TimeTaken.Value : (DateTime?)null,
                        Notes = draft.Notes,
                        SaveSessionId = sessionId
                    };
                    db.Supplements.Add(entry);
                }
                await db.SaveChangesAsync();
            }
            await LoadSupplementNamesAsync();
            SupplementDrafts.Clear();
            SupplementDrafts.Add(new SupplementEntryDraft()); // Reset to one row
            await Shell.Current.DisplayAlert("Saved", "Supplements saved!", "OK");
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public void UseBatch(SupplementBatchVM batch)
        {
            SupplementDrafts.Clear();
            foreach (var item in batch.Items)
            {
                SupplementDrafts.Add(new SupplementEntryDraft
                {
                    Name = item.Name,
                    DosageMg = item.DosageMg,
                    DosageUnit = item.DosageUnit,
                    TimeTaken = item.TimeTaken?.TimeOfDay,
                    Notes = item.Notes
                });
            }
        }

        public async Task LoadRecentBatchesAsync()
        {
            RecentBatches.Clear();
            using (var db = new MigraineTrackerDbContext())
            {
                var lastSessionIds = await db.Supplements
                    .OrderByDescending(s => s.Date)
                    .Select(s => s.SaveSessionId)
                    .Distinct()
                    .Take(5)
                    .ToListAsync();

                foreach (var sessionId in lastSessionIds)
                {
                    var items = await db.Supplements
                        .Where(s => s.SaveSessionId == sessionId)
                        .OrderBy(s => s.Name)
                        .ToListAsync();

                    if (items.Any())
                    {
                        RecentBatches.Add(new SupplementBatchVM
                        {
                            BatchDate = items.First().Date,
                            Items = items
                        });
                    }
                }
            }
        }

        public async Task LoadSupplementNamesAsync()
        {
            SupplementNameSuggestions.Clear();
            using (var db = new MigraineTrackerDbContext())
            {
                var names = await db.Supplements
                    .Where(s => !string.IsNullOrEmpty(s.Name))
                    .Select(s => s.Name!)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToListAsync();

                foreach (var name in names)
                {
                    SupplementNameSuggestions.Add(name);
                }
            }
        }
    }
}
