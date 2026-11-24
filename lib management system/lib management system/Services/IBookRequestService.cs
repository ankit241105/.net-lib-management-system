using lib_management_system.Common;
using lib_management_system.Dtos;

namespace lib_management_system.Services
{
    public interface IBookRequestService
    {
        Task<ServiceResult<BookRequestDto>> CreateAsync(int userId, CreateBookRequestDto dto);
        Task<List<BookRequestDto>> GetMineAsync(int userId);
        Task<List<BookRequestDto>> GetPendingAsync();
        Task<ServiceResult<BookRequestDto>> ApproveAsync(int id);
        Task<ServiceResult<BookRequestDto>> DeclineAsync(int id);
    }
}
