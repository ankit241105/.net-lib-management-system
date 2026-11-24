using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public interface IBorrowRepository
    {
        Task<List<BorrowRecord>> GetAllAsync();            // includes Book + User
        Task<List<BorrowRecord>> GetByUserAsync(int userId); // includes Book
        Task<BorrowRecord?> GetByIdWithBookAsync(int id);  // includes Book
        Task AddAsync(BorrowRecord record);
        Task<int> CountActiveAsync();
        Task<List<BorrowRecord>> GetActiveAsync();         // includes Book + User
        Task<List<BorrowRecord>> GetDueForReminderAsync(DateTime cutoff); // active, due by cutoff, not yet reminded; includes Book + User
        Task SaveChangesAsync();
    }
}
