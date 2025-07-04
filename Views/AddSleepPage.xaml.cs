﻿using System;
using MigraineTracker.Data;
using MigraineTracker.Models;
using Microsoft.Maui.Controls;

namespace MigraineTracker.Views
{
    public partial class AddSleepPage : ContentPage
    {
        public AddSleepPage()
        {
            InitializeComponent();
            // default both pickers to “now”
            var now = DateTime.Now.TimeOfDay;
            var today = DateTime.Today;
            StartDatePicker.Date = today.AddDays(-1);
            EndDatePicker.Date = today;
            StartTimePicker.Time = now;
            EndTimePicker.Time = now;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var start = StartDatePicker.Date + StartTimePicker.Time;
            var end = EndDatePicker.Date + EndTimePicker.Time;

            if (end <= start)
            {
                await DisplayAlert("Error", "End time must be after start time.", "OK");
                return;
            }

            var entry = new SleepEntry
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today,
                SleepStart = start,
                SleepEnd = end,
                Quality = QualityPicker.SelectedItem?.ToString() ?? "Fair",
                Notes = NotesEditor.Text?.Trim() ?? ""
            };

            using var db = new MigraineTrackerDbContext();
            db.Sleeps.Add(entry);
            await db.SaveChangesAsync();

            await DisplayAlert("Saved", $"Sleep saved ({(end - start).TotalHours:F1}h)", "OK");
            await Shell.Current.GoToAsync("..");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
