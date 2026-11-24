using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using lib_management_system.Dtos;
using lib_management_system.Extensions;
using lib_management_system.Services;

namespace lib_management_system.Controllers
{
    [Authorize]
    public class MembershipRequestsController : ApiControllerBase
    {
        private readonly IMembershipRequestService _service;
        public MembershipRequestsController(IMembershipRequestService service) => _service = service;

        // User: request to upgrade to Member (mock ₹199)
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            return ToResponse(await _service.CreateAsync(User.GetUserId()));
        }

        // User: the caller's own membership requests (so the banner can show "pending")
        [Authorize(Roles = "User")]
        [HttpGet("mine")]
        public async Task<ActionResult<List<MembershipRequestDto>>> GetMine()
        {
            return await _service.GetMineAsync(User.GetUserId());
        }

        // Admin: all pending upgrade requests
        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<ActionResult<List<MembershipRequestDto>>> GetPending()
            => await _service.GetPendingAsync();

        // Admin: approve (flips the user's role to Member)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            return ToResponse(await _service.ApproveAsync(id));
        }

        // Admin: decline
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/decline")]
        public async Task<IActionResult> Decline(int id)
        {
            return ToResponse(await _service.DeclineAsync(id));
        }
    }
}
