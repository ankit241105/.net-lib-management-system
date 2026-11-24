namespace lib_management_system.Dtos
{
    // Everything the admin dashboard shows, built only from repository methods we already have.
    public record DashboardStatsDto(
        // Catalog
        int TotalBooks,
        int AvailableBooks,
        int CurrentlyBorrowed,          // books currently out on loan
        int OverdueLoans,               // active loans now past their due date
        // People (by role)
        int TotalUsers,                 // every account
        int TotalRegularUsers,          // role "User"
        int TotalMembers,               // role "Member"
        int TotalAdmins,                // role "Admin"
        // Waiting for an admin decision
        int PendingBookRequests,
        int PendingMembershipRequests,
        // The active loans themselves (for the table)
        List<BorrowRecordDto> ActiveBorrowings);
}
