using Microsoft.EntityFrameworkCore;
using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Domain.Entities;
using TheatreMasterService.Infrastructure.Persistence.Context;

namespace TheatreMasterService.Infrastructure.Repositories
{
    /// <inheritdoc />
    public class ShowRepository(TheatreMasterDbContext context) : IShowRepository
    {
        #region Configuration
        private readonly TheatreMasterDbContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task<IEnumerable<Show>> GetShowsAsync()
        {
            return await _context.Shows.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Show?> GetShowByIdAsync(int id)
        {
            return await _context.Shows.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<Show> CreateShowAsync(Show show)
        {
            await _context.Shows.AddAsync(show);
            await _context.SaveChangesAsync();
            return show;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateShowAsync(Show show)
        {
            _context.Shows.Update(show);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteShowAsync(int id)
        {
            var show = await _context.Shows.FindAsync(id);
            if (show == null)
                return false;
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Show>> GetShowsByMovieId(int movieId)
        {
            return await _context.Shows
                .Where(s => s.MovieId == movieId)
                .ToListAsync();
        }
    }
}