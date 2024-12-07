using FootArt.Data.Entities;
using System.Data.Entity;

namespace FootArt.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=AppDbContext")
        {
        }

        public DbSet<StudyData> StudyData { get; set; }
        public DbSet<StudyDataPoint> StudyDataPoint { get; set; }

        public DbSet<FaceRatioData> FaceRatioData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
