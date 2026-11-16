using Microsoft.EntityFrameworkCore;
using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Domain.Entities;
using TheatreMasterService.Infrastructure.Persistence.Context;

namespace TheatreMasterService.Infrastructure.Repositories
{
    /// <inheritdoc />
    public class ScreenRepository(TheatreMasterDbContext context) : IScreenRepository
    {
        #region Configuration
        private readonly TheatreMasterDbContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task<Screen> CreateScreenAsync(Screen screen)
        {
            _context.Screens.Add(screen);
            await _context.SaveChangesAsync();
            return screen;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteScreenAsync(int id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null)
                return false;
            _context.Screens.Remove(screen);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<Screen?> GetScreenByIdAsync(int id)
        {
            return await _context.Screens.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Screen>> GetScreensAsync()
        {
            return await _context.Screens.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<bool> UpdateScreenAsync(Screen screen)
        {
            _context.Screens.Update(screen);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}