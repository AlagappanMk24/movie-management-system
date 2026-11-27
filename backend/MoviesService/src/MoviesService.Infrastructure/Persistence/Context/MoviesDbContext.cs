using Microsoft.EntityFrameworkCore;
using MoviesService.Domain.Entities;

namespace MoviesService.Infrastructure.Persistence.Context
{
    /// <summary>
    /// Entity Framework Core database context for managing movie persistence and entity configurations.
    /// </summary>
    public class MoviesDbContext(DbContextOptions<MoviesDbContext> options) : DbContext(options)
    {
        /// <summary>
        /// Gets or sets the DbSet for movie entities.
        /// </summary>
        public DbSet<Movies> Movies { get; set; }

        /// <summary>
        /// Configures entity relationships, primary keys, and database indexes.
        /// </summary>
        /// <param name="modelBuilder">The model builder instance.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movies>(e =>
            {
                e.HasKey(m => m.Id);
                e.HasIndex(m => m.Title);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}