using Microsoft.EntityFrameworkCore;
using lib_management_system.Data;
using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public class MembershipRequestRepository : IMembershipRequestRepository
    {
        private readonly LibraryDbContext _context;
        public MembershipRequestRepository(LibraryDbContext context) => _context = context;

        public async Task<MembershipRequest?> GetByIdAsync(int id) =>
            await _context.MembershipRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<List<MembershipRequest>> GetPendingAsync() =>
            await _context.MembershipRequests
                .Where(r => r.Status == "Pending")
                .Include(r => r.User)
                .OrderBy(r => r.RequestedDate)
                .ToListAsync();

        public async Task<List<MembershipRequest>> GetByUserAsync(int userId) =>
            await _context.MembershipRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestedDate)
                .ToListAsync();

        public async Task<bool> HasPendingAsync(int userId) =>
            await _context.MembershipRequests.AnyAsync(r =>
                r.UserId == userId && r.Status == "Pending");

        public async Task AddAsync(MembershipRequest request) => await _context.MembershipRequests.AddAsync(request);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
