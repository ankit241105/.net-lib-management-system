using lib_management_system.Common;
using lib_management_system.Dtos;

namespace lib_management_system.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<string>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<string>> RegisterAdminAsync(RegisterRequest request, string? creator);
        Task<ServiceResult<AuthTokens>> LoginAsync(LoginRequest request);
        Task<ServiceResult<AuthTokens>> RefreshAsync(string? refreshToken);
        Task LogoutAsync(string? refreshToken);
    }
}
