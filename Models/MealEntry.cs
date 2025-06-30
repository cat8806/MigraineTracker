using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraineTracker.Models
{
    public class MealEntry
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }            // e.g. 2025-07-02
        public string MealType { get; set; }          // “Breakfast”, “Lunch”, etc.
        public DateTime? Time { get; set; }           // Today’s date + time of meal
        public string FoodItems { get; set; }         // e.g. “Oatmeal, Coffee”
        public bool ContainsTrigger { get; set; }     // true if flagged
        public string? TriggerNotes { get; set; }      // e.g. “Dairy”
        public string? Notes { get; set; }             // optional extra notes
    }
}
