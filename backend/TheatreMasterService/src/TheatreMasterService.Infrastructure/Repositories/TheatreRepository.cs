using Microsoft.EntityFrameworkCore;
using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Domain.Entities;
using TheatreMasterService.Infrastructure.Persistence.Context;

namespace TheatreMasterService.Infrastructure.Repositories
{
    /// <inheritdoc />
    public class TheatreRepository(TheatreMasterDbContext context) : ITheatreRepository
    {
        #region Configuration
        private readonly TheatreMasterDbContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task<Theatre> CreateTheatreAsync(Theatre theatre)
        {
            theatre.Created = DateTime.Now;
            theatre.Modified = DateTime.Now;
            await _context.Theatres.AddAsync(theatre);
            await _context.SaveChangesAsync();
            return theatre;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Theatre>> GetTheatresAsync()
        {
            return await _context.Theatres.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Theatre?> GetTheatreByIdAsync(int id)
        {
            return await _context.Theatres.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateTheatreAsync(Theatre theatre)
        {
            var existingTheatre = await _context.Theatres.FindAsync(theatre.TheatreId);
            if (existingTheatre == null)
                return false;
            existingTheatre.TheatreName = theatre.TheatreName;
            existingTheatre.City = theatre.City;
            existingTheatre.Modified = DateTime.Now;
            _context.Theatres.Update(existingTheatre);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteTheatreAsync(int id)
        {
            var existingTheatre = await _context.Theatres.FindAsync(id);
            if (existingTheatre == null)
                return false;
            _context.Theatres.Remove(existingTheatre);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}