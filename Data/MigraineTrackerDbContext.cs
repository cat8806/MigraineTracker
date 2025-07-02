using Microsoft.EntityFrameworkCore;
using MigraineTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigraineTracker.Data
{
    public class MigraineTrackerDbContext : DbContext
    {
        public DbSet<MigraineEntry> Migraines { get; set; }
        public DbSet<SupplementEntry> Supplements { get; set; }
        public DbSet<MealEntry> Meals { get; set; }
        public DbSet<WaterIntakeEntry> WaterIntakes { get; set; }
        public DbSet<SleepEntry> Sleeps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "migraine.db");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
