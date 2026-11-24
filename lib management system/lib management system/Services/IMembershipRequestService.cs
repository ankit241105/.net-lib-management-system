using lib_management_system.Common;
using lib_management_system.Dtos;

namespace lib_management_system.Services
{
    public interface IMembershipRequestService
    {
        Task<ServiceResult<MembershipRequestDto>> CreateAsync(int userId);
        Task<List<MembershipRequestDto>> GetMineAsync(int userId);
        Task<List<MembershipRequestDto>> GetPendingAsync();
        Task<ServiceResult<MembershipRequestDto>> ApproveAsync(int id);
        Task<ServiceResult<MembershipRequestDto>> DeclineAsync(int id);
    }
}
