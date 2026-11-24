using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using lib_management_system.Dtos;
using lib_management_system.Services;

namespace lib_management_system.Controllers
{
    [Authorize]
    public class BooksController : ApiControllerBase
    {
        private readonly IBookService _service;
        public BooksController(IBookService service) => _service = service;

        // Cursor-based (keyset) pagination.
        //   cursor  = the Id returned as NextCursor by the previous page (omit for page 1)
        //   limit   = page size (clamped to 1..100 in the service; default 20)
        // Response carries the items, the NextCursor to pass back, and HasMore.
        [HttpGet]
        public async Task<ActionResult<PagedBooksDto>> GetBooks(
            int? cursor = null, int limit = 20, string? search = null, bool? available = null)
            => await _service.GetBooksPageAsync(cursor, limit, search, available);

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _service.GetBookAsync(id);
            return book is null ? NotFound() : book;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto dto)
        {
            var created = await _service.CreateBookAsync(dto);
            return CreatedAtAction(nameof(GetBook), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto dto)
            => await _service.UpdateBookAsync(id, dto) ? NoContent() : NotFound();

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
            => await _service.DeleteBookAsync(id) ? NoContent() : NotFound();
    }
}