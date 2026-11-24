using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public interface IMembershipRequestRepository
    {
        Task<MembershipRequest?> GetByIdAsync(int id);            // includes User
        Task<List<MembershipRequest>> GetPendingAsync();          // includes User
        Task<List<MembershipRequest>> GetByUserAsync(int userId);
        Task<bool> HasPendingAsync(int userId);
        Task AddAsync(MembershipRequest request);
        Task SaveChangesAsync();
    }
}
