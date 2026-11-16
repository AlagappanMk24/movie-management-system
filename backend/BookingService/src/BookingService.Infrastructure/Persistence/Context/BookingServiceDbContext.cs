using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Persistence.Context
{
    /// <summary>
    /// Entity Framework database context for managing booking service persistence entities.
    /// </summary>
    public class BookingServiceDbContext(DbContextOptions<BookingServiceDbContext> options) : DbContext(options)
    {
        /// <summary>Gets or sets the bookings table DbSet.</summary>
        public DbSet<Booking> Bookings { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.BookingId);
                entity.Property(b => b.UserId).IsRequired();
                entity.Property(b => b.ShowId).IsRequired();
            });
        }
    }
}