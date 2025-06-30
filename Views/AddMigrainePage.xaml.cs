using System;
using MigraineTracker.Data;
using MigraineTracker.Models;


namespace MigraineTracker.Views
{
    public partial class AddMigrainePage : ContentPage
    {
        public AddMigrainePage()
        {
            InitializeComponent();
            var now = DateTime.Now.TimeOfDay;
            StartTimePicker.Time = now;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Gather values from the UI
            var migraine = new MigraineEntry
            {
                Id = Guid.NewGuid(),
                Date = DatePicker.Date,
                StartTime = DatePicker.Date + StartTimePicker.Time,
                EndTime = DatePicker.Date + EndTimePicker.Time,
                Severity = (int)SeveritySlider.Value,
                Triggers = TriggersEntry.Text ?? "",
                Notes = NotesEditor.Text ?? ""
            };

            // Save to SQLite using DbContext
            using (var db = new MigraineTrackerDbContext())
            {
                db.Migraines.Add(migraine);
                await db.SaveChangesAsync();
            }

            // Navigate back
            await DisplayAlert("Saved", "Migraine entry added!", "OK");
            await Navigation.PopAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
