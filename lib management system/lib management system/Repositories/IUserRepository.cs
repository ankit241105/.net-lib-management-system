using lib_management_system.Models;

namespace lib_management_system.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<int> CountByRoleAsync(string role);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
