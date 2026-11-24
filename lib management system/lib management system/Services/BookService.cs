using lib_management_system.Dtos;
using lib_management_system.Models;
using lib_management_system.Repositories;

namespace lib_management_system.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repo;   // depends on the INTERFACE, not EF
        public BookService(IBookRepository repo) => _repo = repo;

        public async Task<List<BookDto>> GetBooksAsync(string? search, bool? available, string? sortBy)
        {
            var books = await _repo.GetAllAsync(search, available, sortBy);
            return books.Select(b => b.ToDto()).ToList();
        }

        public async Task<PagedBooksDto> GetBooksPageAsync(int? cursor, int limit, string? search, bool? available)
        {
            // Never trust a client-supplied page size blindly: clamp it to a sane range.
            limit = Math.Clamp(limit, 1, 100);

            var (rows, hasMore) = await _repo.GetPageAsync(cursor, limit, search, available);
            var items = rows.Select(b => b.ToDto()).ToList();

            // The next cursor is simply the Id of the last row on this page.
            int? nextCursor = hasMore && items.Count > 0 ? items[^1].Id : null;

            return new PagedBooksDto(items, nextCursor, hasMore);
        }

        public async Task<BookDto?> GetBookAsync(int id)
        {
            var book = await _repo.GetByIdAsync(id);
            return book is null ? null : book.ToDto();
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
        {
            var book = new Book { Title = dto.Title, Author = dto.Author, IsAvailable = true };
            await _repo.AddAsync(book);
            await _repo.SaveChangesAsync();
            return book.ToDto();
        }

        public async Task<bool> UpdateBookAsync(int id, UpdateBookDto dto)
        {
            var book = await _repo.GetByIdAsync(id);
            if (book is null) return false;

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.IsAvailable = dto.IsAvailable;
            _repo.Update(book);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _repo.GetByIdAsync(id);
            if (book is null) return false;

            _repo.Remove(book);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}