using lib_management_system.Dtos;
using lib_management_system.Repositories;

namespace lib_management_system.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IBookRepository _bookRepo;
        private readonly IUserRepository _userRepo;
        private readonly IBorrowRepository _borrowRepo;
        private readonly IBookRequestRepository _bookRequestRepo;
        private readonly IMembershipRequestRepository _membershipRequestRepo;

        public DashboardService(
            IBookRepository bookRepo,
            IUserRepository userRepo,
            IBorrowRepository borrowRepo,
            IBookRequestRepository bookRequestRepo,
            IMembershipRequestRepository membershipRequestRepo)
        {
            _bookRepo = bookRepo;
            _userRepo = userRepo;
            _borrowRepo = borrowRepo;
            _bookRequestRepo = bookRequestRepo;
            _membershipRequestRepo = membershipRequestRepo;
        }

        public async Task<DashboardStatsDto> GetStatsAsync()
        {
            // Catalog
            var totalBooks = await _bookRepo.CountAsync();
            var availableBooks = await _bookRepo.CountAvailableAsync();
            var currentlyBorrowed = await _borrowRepo.CountActiveAsync();

            // People, counted per role
            var regularUsers = await _userRepo.CountByRoleAsync("User");
            var members = await _userRepo.CountByRoleAsync("Member");
            var admins = await _userRepo.CountByRoleAsync("Admin");

            // Items waiting for an admin decision
            var pendingBookRequests = (await _bookRequestRepo.GetPendingAsync()).Count;
            var pendingMembershipRequests = (await _membershipRequestRepo.GetPendingAsync()).Count;

            // Active loans (also used to derive the overdue count)
            var active = await _borrowRepo.GetActiveAsync();
            var overdueLoans = active.Count(r => r.DueDate < DateTime.UtcNow);

            return new DashboardStatsDto(
                totalBooks,
                availableBooks,
                currentlyBorrowed,
                overdueLoans,
                regularUsers + members + admins,
                regularUsers,
                members,
                admins,
                pendingBookRequests,
                pendingMembershipRequests,
                active.Select(r => r.ToDto()).ToList());
        }
    }
}
