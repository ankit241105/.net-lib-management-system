using lib_management_system.Dtos;

namespace lib_management_system.Services
{
    public interface IBookService
    {
        Task<List<BookDto>> GetBooksAsync(string? search, bool? available, string? sortBy);
        Task<PagedBooksDto> GetBooksPageAsync(int? cursor, int limit, string? search, bool? available);
        Task<BookDto?> GetBookAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto dto);
        Task<bool> UpdateBookAsync(int id, UpdateBookDto dto);
        Task<bool> DeleteBookAsync(int id);
    }
}