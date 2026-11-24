using Microsoft.EntityFrameworkCore;
using lib_management_system.Data;
using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly LibraryDbContext _context;
        public RefreshTokenRepository(LibraryDbContext context) => _context = context;

        public async Task<RefreshToken?> GetByTokenAsync(string token) =>
            await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);

        public async Task AddAsync(RefreshToken token) => await _context.RefreshTokens.AddAsync(token);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
