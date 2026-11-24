using Microsoft.EntityFrameworkCore;
using lib_management_system.Data;
using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public class BorrowRepository : IBorrowRepository
    {
        private readonly LibraryDbContext _context;
        public BorrowRepository(LibraryDbContext context) => _context = context;

        public async Task<List<BorrowRecord>> GetAllAsync() =>
            await _context.BorrowRecords
                .Include(br => br.Book)
                .Include(br => br.User)
                .ToListAsync();

        public async Task<List<BorrowRecord>> GetByUserAsync(int userId) =>
            await _context.BorrowRecords
                .Where(br => br.UserId == userId)
                .Include(br => br.Book)
                .ToListAsync();

        public async Task<BorrowRecord?> GetByIdWithBookAsync(int id) =>
            await _context.BorrowRecords
                .Include(br => br.Book)
                .FirstOrDefaultAsync(br => br.Id == id);

        public async Task AddAsync(BorrowRecord record) => await _context.BorrowRecords.AddAsync(record);

        public async Task<int> CountActiveAsync() =>
            await _context.BorrowRecords.CountAsync(br => br.ReturnDate == null);

        public async Task<List<BorrowRecord>> GetActiveAsync() =>
            await _context.BorrowRecords
                .Where(br => br.ReturnDate == null)
                .Include(br => br.Book)
                .Include(br => br.User)
                .ToListAsync();

        public async Task<List<BorrowRecord>> GetDueForReminderAsync(DateTime cutoff) =>
            await _context.BorrowRecords
                .Where(br => br.ReturnDate == null
                          && br.ReminderSentAt == null
                          && br.DueDate <= cutoff)
                .Include(br => br.Book)
                .Include(br => br.User)
                .ToListAsync();

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
