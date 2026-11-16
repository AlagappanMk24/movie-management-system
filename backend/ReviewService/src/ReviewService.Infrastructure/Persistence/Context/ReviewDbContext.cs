using Microsoft.EntityFrameworkCore;
using ReviewService.Domain.Entities;

namespace ReviewService.Infrastructure.Persistence.Context
{
    /// <summary>
    /// Entity Framework database context for managing review service persistence entities.
    /// </summary>
    public class ReviewDbContext(DbContextOptions<ReviewDbContext> options) : DbContext(options)
    {
        /// <summary>Gets or sets the reviews table DbSet.</summary>
        public DbSet<Review> Reviews { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.MovieId, r.UserId })
                .IsUnique();
        }
    }
}