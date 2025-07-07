using System;
using MigraineTracker.Data;
using MigraineTracker.Models;
using Microsoft.Maui.Controls;

namespace MigraineTracker.Views
{
    public partial class AddDrinkPage : ContentPage
    {
        public AddDrinkPage()
        {
            InitializeComponent();
            // default the TimePicker to the current time
            TimePicker.Time = DateTime.Now.TimeOfDay;
        }

        // Quick-add buttons (350/700/1000 mL)
        private async void OnQuickAddClicked(object sender, EventArgs e)
        {
            if (!(sender is Button btn) ||
                !int.TryParse(btn.CommandParameter?.ToString(), out int volume))
            {
                return;
            }

            var entry = new DrinkEntry
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                DrinkType = DrinkTypeEntry.Text?.Trim() ?? "Water",
                VolumeMl = volume,
                Time = DateTime.Now   // ← use now directly
            };

            using var db = new MigraineTrackerDbContext();
            db.Drinks.Add(entry);
            await db.SaveChangesAsync();

            await DisplayAlert("Saved", $"{volume} mL added!", "OK");
            await Shell.Current.GoToAsync("..");
        }

        // Custom-amount save
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (!int.TryParse(CustomEntry.Text, out int volume) || volume <= 0)
            {
                await DisplayAlert("Error", "Please enter a valid amount in mL.", "OK");
                return;
            }

            var entry = new DrinkEntry
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                DrinkType = DrinkTypeEntry.Text?.Trim() ?? "Water",
                VolumeMl = volume,
                Time = DateTime.Today + TimePicker.Time
            };

            using var db = new MigraineTrackerDbContext();
            db.Drinks.Add(entry);
            await db.SaveChangesAsync();

            await DisplayAlert("Saved", $"{volume} mL added!", "OK");
            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
