using lib_management_system.Dtos;

namespace lib_management_system.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsAsync();
    }
}
