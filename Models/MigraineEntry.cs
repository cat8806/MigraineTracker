using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraineTracker.Models
{
    public class MigraineEntry
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Severity { get; set; } // 1–10 (or whatever scale you use)
        public string? Triggers { get; set; } // e.g. "Stress;Lack of sleep"
        public string? Notes { get; set; }
    }
}
