using Microsoft.EntityFrameworkCore;
using lib_management_system.Data;
using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LibraryDbContext _context;
        public UserRepository(LibraryDbContext context) => _context = context;

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<bool> ExistsByUsernameAsync(string username) =>
            await _context.Users.AnyAsync(u => u.Username == username);

        public async Task<int> CountByRoleAsync(string role) =>
            await _context.Users.CountAsync(u => u.Role == role);

        public async Task AddAsync(User user) => await _context.Users.AddAsync(user);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
