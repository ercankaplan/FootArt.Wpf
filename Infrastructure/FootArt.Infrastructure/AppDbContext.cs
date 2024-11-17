using FootArt.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootArt.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<StudyData> StudyData { get; set; }
        public DbSet<StudyDataPoint> StudyDataPoint { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    }
}
