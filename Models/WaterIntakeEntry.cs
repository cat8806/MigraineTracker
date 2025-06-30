using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraineTracker.Models
{
    public class WaterIntakeEntry
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int VolumeMl { get; set; } // e.g., 250 for 250ml
        public DateTime? Time { get; set; }
    }
}
