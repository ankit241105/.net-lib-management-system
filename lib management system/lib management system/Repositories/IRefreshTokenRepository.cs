using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);   // includes User
        Task AddAsync(RefreshToken token);
        Task SaveChangesAsync();
    }
}
