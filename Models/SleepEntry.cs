using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraineTracker.Models
{
    public class SleepEntry
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? SleepStart { get; set; }
        public DateTime? SleepEnd { get; set; }
        public double DurationHours
        {
            get
            {
                if (SleepStart.HasValue && SleepEnd.HasValue)
                    return (SleepEnd.Value - SleepStart.Value).TotalHours;
                return 0;
            }
        }
        public string Quality { get; set; } // "Good", "Fair", "Poor"
        public string Notes { get; set; } // e.g., "Snoring; Low blood oxygen"
    }
}
