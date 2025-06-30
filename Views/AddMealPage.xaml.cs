using System;
using MigraineTracker.Data;
using MigraineTracker.Models;
using Microsoft.Maui.Controls;

namespace MigraineTracker.Views
{
    public partial class AddMealPage : ContentPage
    {
        public AddMealPage()
        {
            InitializeComponent();
            var now = DateTime.Now.TimeOfDay;
            TimePicker.Time = now;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var meal = new MealEntry
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                MealType = MealTypePicker.SelectedItem?.ToString() ?? "Meal",
                Time = DateTime.Today + TimePicker.Time,
                FoodItems = FoodItemsEditor.Text?.Trim() ?? "",
                ContainsTrigger = TriggerSwitch.IsToggled,
                TriggerNotes = TriggerSwitch.IsToggled
                                 ? TriggerNotesEntry.Text?.Trim()
                                 : null,
                Notes = NotesEditor.Text?.Trim() ?? ""
            };

            using var db = new MigraineTrackerDbContext();
            db.Meals.Add(meal);
            await db.SaveChangesAsync();

            await DisplayAlert("Saved", "Meal entry added!", "OK");
            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
