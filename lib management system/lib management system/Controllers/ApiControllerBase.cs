using Microsoft.AspNetCore.Mvc;
using lib_management_system.Common;

namespace lib_management_system.Controllers
{
    // Shared base for every API controller: carries the common [ApiController]/[Route]
    // and turns a ServiceResult into an HTTP response in one place, so all endpoints
    // report success and failure the same way.
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult ToResponse<T>(ServiceResult<T> result) =>
            result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, result.Error);
    }
}
