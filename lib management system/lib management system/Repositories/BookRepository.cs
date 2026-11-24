using Microsoft.EntityFrameworkCore;
using lib_management_system.Data;
using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;
        public BookRepository(LibraryDbContext context) => _context = context;

        public async Task<List<Book>> GetAllAsync(string? search, bool? available, string? sortBy)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));

            if (available.HasValue)
                query = query.Where(b => b.IsAvailable == available.Value);

            query = sortBy?.ToLower() switch
            {
                "title" => query.OrderBy(b => b.Title),
                "author" => query.OrderBy(b => b.Author),
                _ => query.OrderBy(b => b.Id)
            };

            return await query.ToListAsync();
        }

        public async Task<(List<Book> Items, bool HasMore)> GetPageAsync(
            int? cursor, int limit, string? search, bool? available)
        {
            var query = _context.Books.AsQueryable();

            // Same filters as the normal list — they don't affect ordering.
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));

            if (available.HasValue)
                query = query.Where(b => b.IsAvailable == available.Value);

            // The cursor is the last Id the client already received. We want only the rows
            // that come AFTER it. This is the heart of keyset pagination: a "seek" by value
            // instead of an OFFSET that counts and throws away rows.
            if (cursor.HasValue)
                query = query.Where(b => b.Id > cursor.Value);

            // Order by the cursor column so "Id > cursor" slices a contiguous window, and
            // ask for one extra row: if it comes back, we know another page exists.
            var rows = await query
                .OrderBy(b => b.Id)
                .Take(limit + 1)
                .ToListAsync();

            var hasMore = rows.Count > limit;
            if (hasMore)
                rows.RemoveAt(rows.Count - 1);   // drop the sentinel extra row

            return (rows, hasMore);
        }

        public async Task<Book?> GetByIdAsync(int id) => await _context.Books.FindAsync(id);
        public async Task AddAsync(Book book) => await _context.Books.AddAsync(book);
        public void Update(Book book) => _context.Books.Update(book);
        public void Remove(Book book) => _context.Books.Remove(book);
        public async Task<bool> ExistsAsync(int id) => await _context.Books.AnyAsync(b => b.Id == id);
        public async Task<int> CountAsync() => await _context.Books.CountAsync();
        public async Task<int> CountAvailableAsync() => await _context.Books.CountAsync(b => b.IsAvailable);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}