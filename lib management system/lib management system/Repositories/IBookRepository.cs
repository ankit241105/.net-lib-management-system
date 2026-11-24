using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllAsync(string? search, bool? available, string? sortBy);

        // Keyset (cursor) pagination: returns up to `limit` rows whose Id is greater than
        // `cursor`, plus a flag saying whether at least one more row exists after them.
        Task<(List<Book> Items, bool HasMore)> GetPageAsync(
            int? cursor, int limit, string? search, bool? available);
        Task<Book?> GetByIdAsync(int id);
        Task AddAsync(Book book);
        void Update(Book book);
        void Remove(Book book);
        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync();
        Task<int> CountAvailableAsync();
        Task SaveChangesAsync();
    }
}