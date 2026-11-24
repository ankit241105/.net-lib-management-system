namespace lib_management_system.Dtos
{
    // Request: a Member asks to borrow a book (their identity comes from the JWT, not the body).
    public record CreateBookRequestDto(int BookId);

    // Response: a book request with the book title + requester username flattened in.
    public record BookRequestDto(
        int Id,
        int BookId,
        string? BookTitle,
        int UserId,
        string? Username,
        string Status,
        DateTime RequestedDate,
        DateTime? DecisionDate);
}
