using lib_management_system.Common;
using lib_management_system.Models;

namespace lib_management_system.Dtos
{
    // All entity -> DTO conversions in one place, so mapping is done the same way everywhere.
    public static class MappingExtensions
    {
        public static BookDto ToDto(this Book b) =>
            new(b.Id, b.Title, b.Author, b.IsAvailable);

        // For a returned book the stored fine is final; for a book still out the fine is
        // shown accruing live (today vs. due date).
        public static BorrowRecordDto ToDto(this BorrowRecord r) =>
            new(r.Id,
                r.BorrowDate,
                r.DueDate,
                r.ReturnDate,
                r.ReturnDate.HasValue ? r.Fine : Fines.Calculate(r.DueDate, DateTime.UtcNow),
                r.BookId,
                r.Book?.ToDto(),
                r.UserId,
                r.User?.Username);

        public static BookRequestDto ToDto(this BookRequest r) =>
            new(r.Id, r.BookId, r.Book?.Title, r.UserId, r.User?.Username,
                r.Status, r.RequestedDate, r.DecisionDate);

        public static MembershipRequestDto ToDto(this MembershipRequest r) =>
            new(r.Id, r.UserId, r.User?.Username, r.Amount,
                r.Status, r.RequestedDate, r.DecisionDate);
    }
}
