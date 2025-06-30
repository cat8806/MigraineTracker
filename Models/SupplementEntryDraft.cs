using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraineTracker.Models
{
    public class SupplementEntryDraft
    {
        public string Name { get; set; }
        public double DosageMg { get; set; }
        public string DosageUnit { get; set; }
        public TimeSpan? TimeTaken { get; set; }
        public string Notes { get; set; }
    }
}
