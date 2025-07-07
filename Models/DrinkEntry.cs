using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraineTracker.Models
{
    /// <summary>
    /// Represents a drink that was consumed.
    /// </summary>
    public class DrinkEntry
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        /// <summary>
        /// Name of the drink e.g. "Water", "Coke".
        /// </summary>
        public string DrinkType { get; set; } = "Water";
        /// <summary>
        /// Volume in milliliters.
        /// </summary>
        public int VolumeMl { get; set; }
        public DateTime? Time { get; set; }
    }
}
