using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Context
{
    /// <summary>
    /// Entity Framework database context for user management.
    /// </summary>
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        /// <summary>Gets or sets the users database set.</summary>
        public DbSet<User> Users { get; set; }

        /// <summary>Configures database entity relationships and constraints.</summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}