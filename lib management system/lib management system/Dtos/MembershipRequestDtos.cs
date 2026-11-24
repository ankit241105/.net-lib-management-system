namespace lib_management_system.Dtos
{
    // Response: a user's membership-upgrade request.
    public record MembershipRequestDto(
        int Id,
        int UserId,
        string? Username,
        decimal Amount,
        string Status,
        DateTime RequestedDate,
        DateTime? DecisionDate);
}
