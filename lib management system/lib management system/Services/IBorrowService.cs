using lib_management_system.Common;
using lib_management_system.Dtos;

namespace lib_management_system.Services
{
    public interface IBorrowService
    {
        Task<List<BorrowRecordDto>> GetRecordsAsync();            // admin: all loans
        Task<List<BorrowRecordDto>> GetMyRecordsAsync(int userId); // member: own loans
        Task<ServiceResult<BorrowRecordDto>> ReturnAsync(int id);  // admin: mark returned
    }
}
