using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public interface IBookRequestRepository
    {
        Task<BookRequest?> GetByIdAsync(int id);              // includes Book + User
        Task<List<BookRequest>> GetPendingAsync();            // includes Book + User
        Task<List<BookRequest>> GetByUserAsync(int userId);   // includes Book
        Task<bool> HasPendingForBookAsync(int userId, int bookId);
        Task AddAsync(BookRequest request);
        Task SaveChangesAsync();
    }
}
