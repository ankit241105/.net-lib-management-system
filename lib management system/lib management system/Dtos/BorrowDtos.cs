namespace lib_management_system.Dtos
{
    // Response: a borrow record with its book nested + the borrower's account info,
    // the due date, and the (live or final) late fine.
    public record BorrowRecordDto(
        int Id,
        DateTime BorrowDate,
        DateTime DueDate,
        DateTime? ReturnDate,
        decimal Fine,
        int BookId,
        BookDto? Book,
        int UserId,
        string? Username);
}
