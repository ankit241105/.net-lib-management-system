using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using lib_management_system.Dtos;
using lib_management_system.Services;

namespace lib_management_system.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : ApiControllerBase
    {
        private readonly IDashboardService _service;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService service, ILogger<DashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            _logger.LogInformation("Dashboard stats accessed by {User}", User.Identity?.Name);
            return await _service.GetStatsAsync();
        }
    }
}
