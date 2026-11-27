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

        public DbSet<BookingSeat> BookingSeats { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.UserId).IsRequired();
                entity.Property(b => b.ShowId).IsRequired();
                entity.Property(b => b.SeatCount).IsRequired();
                entity.Property(b => b.PaymentMethod).HasMaxLength(100);
                entity.Property(b => b.PaymentStatus).HasMaxLength(100);

                entity.HasIndex(b => b.UserId).HasDatabaseName("IX_Bookings_UserId");
                entity.HasIndex(b => b.ShowId).HasDatabaseName("IX_Bookings_ShowId");
            });

            modelBuilder.Entity<BookingSeat>(entity =>
            {
                entity.HasKey(bs => bs.Id);

                entity.Property(bs => bs.BookingId).IsRequired();
                entity.Property(bs => bs.ShowId).IsRequired();
                entity.Property(bs => bs.SeatNo)
                      .IsRequired()
                      .HasMaxLength(20);
                entity.Property(bs => bs.Price).IsRequired();

                entity.HasIndex(bs => bs.BookingId)
                       .HasDatabaseName("IX_BookingSeats_BookingId");

                entity.HasIndex(bs => bs.BookingId);

                // ✅ UNIQUE constraint on (ShowId, SeatNo)
                entity.HasIndex(bs => new { bs.ShowId, bs.SeatNo })
                  .IsUnique()
                  .HasFilter("[IsDeleted] = 0")
                  .HasDatabaseName("UQ_BookingSeats_Show_Seat_Active");

                entity.HasOne<Booking>()
                      .WithMany()
                      .HasForeignKey(bs => bs.BookingId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}