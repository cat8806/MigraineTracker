using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraineTracker.Models
{
    public class SupplementEntry
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string? Name { get; set; } // e.g., "Fish Oil"
        public double DosageMg { get; set; } // e.g., 500 for 500mg
        public string? DosageUnit { get; set; } // e.g., "mg", "IU", "capsule"
        public DateTime? TimeTaken { get; set; }
        public string? Notes { get; set; }

        public Guid SaveSessionId { get; set; }
    }
}
