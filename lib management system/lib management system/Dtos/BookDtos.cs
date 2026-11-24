namespace lib_management_system.Dtos
{
    // What the API returns
    public record BookDto(int Id, string Title, string Author, bool IsAvailable);

    // What the client sends to create (note: no Id, no IsAvailable — server decides)
    public record CreateBookDto(string Title, string Author);

    // What the client sends to update
    public record UpdateBookDto(string Title, string Author, bool IsAvailable);

    // One page of books plus the cursor the client sends back to get the next page.
    // NextCursor is null when there are no more rows (HasMore == false).
    public record PagedBooksDto(List<BookDto> Items, int? NextCursor, bool HasMore);
}