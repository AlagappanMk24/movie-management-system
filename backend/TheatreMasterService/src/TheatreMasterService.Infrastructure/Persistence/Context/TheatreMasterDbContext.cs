using Microsoft.EntityFrameworkCore;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Infrastructure.Persistence.Context
{
    /// <summary>
    /// Entity Framework database context for managing theatre master service persistence entities.
    /// </summary>
    public class TheatreMasterDbContext(DbContextOptions<TheatreMasterDbContext> options) : DbContext(options)
    {
        /// <summary>Gets or sets the theatres table DbSet.</summary>
        public DbSet<Theatre> Theatres { get; set; }

        /// <summary>Gets or sets the screens table DbSet.</summary>
        public DbSet<Screen> Screens { get; set; }

        /// <summary>Gets or sets the shows table DbSet.</summary>
        public DbSet<Show> Shows { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =========================
            // THEATRE
            // =========================
            modelBuilder.Entity<Theatre>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.HasIndex(t => t.TheatreName)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0");

                entity.HasMany(t => t.Screens)
                      .WithOne(s => s.Theatre)
                      .HasForeignKey(s => s.TheatreId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // SCREEN
            // =========================
            modelBuilder.Entity<Screen>(entity =>
            {
                entity.HasKey(s => s.Id);

                // One Theatre -> Many Screens (unique per active screen)
                entity.HasIndex(s => new { s.TheatreId, s.ScreenName })
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0");

                entity.HasMany(s => s.Shows)
                      .WithOne(sh => sh.Screen)
                      .HasForeignKey(sh => sh.ScreenId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // SHOW
            // =========================
            modelBuilder.Entity<Show>(entity =>
            {
                entity.HasKey(s => s.Id);

                // Prevent duplicate shows
                entity.HasIndex(s => new
                {
                    s.MovieId,
                    s.ScreenId,
                    s.ShowDate,
                    s.ShowTime
                }).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}