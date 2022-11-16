using Microsoft.EntityFrameworkCore;
using victoria_tap.Entities;

namespace victoria_tap.Repositories
{
    public class VictoriaDbContext : DbContext
    {
        protected override void OnConfiguring
        (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "VictoriaTapDb");
        }
        public DbSet<Dispenser> Dispensers { get; set; }
        public DbSet<DispenserInfo> DispensersInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Usages>(
                    eb =>
                    {
                        eb.Property(u => u.Id)
                            .ValueGeneratedOnAdd();
                        eb.Property(u => u.ClosedAt)
                            .IsRequired(false);
                    }
                 );

            modelBuilder.Entity<DispenserInfo>().HasMany(di => di.Usages);
        }
    }         
}
