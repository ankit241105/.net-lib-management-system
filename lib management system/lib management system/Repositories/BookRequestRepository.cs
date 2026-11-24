using Microsoft.EntityFrameworkCore;
using lib_management_system.Data;
using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public class BookRequestRepository : IBookRequestRepository
    {
        private readonly LibraryDbContext _context;
        public BookRequestRepository(LibraryDbContext context) => _context = context;

        public async Task<BookRequest?> GetByIdAsync(int id) =>
            await _context.BookRequests
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<List<BookRequest>> GetPendingAsync() =>
            await _context.BookRequests
                .Where(r => r.Status == "Pending")
                .Include(r => r.Book)
                .Include(r => r.User)
                .OrderBy(r => r.RequestedDate)
                .ToListAsync();

        public async Task<List<BookRequest>> GetByUserAsync(int userId) =>
            await _context.BookRequests
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .OrderByDescending(r => r.RequestedDate)
                .ToListAsync();

        public async Task<bool> HasPendingForBookAsync(int userId, int bookId) =>
            await _context.BookRequests.AnyAsync(r =>
                r.UserId == userId && r.BookId == bookId && r.Status == "Pending");

        public async Task AddAsync(BookRequest request) => await _context.BookRequests.AddAsync(request);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
