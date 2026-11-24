using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using lib_management_system.Dtos;
using lib_management_system.Extensions;
using lib_management_system.Services;

namespace lib_management_system.Controllers
{
    [Authorize]
    public class BookRequestsController : ApiControllerBase
    {
        private readonly IBookRequestService _service;
        public BookRequestsController(IBookRequestService service) => _service = service;

        // Member: request to borrow a book
        [Authorize(Roles = "Member")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookRequestDto dto)
        {
            return ToResponse(await _service.CreateAsync(User.GetUserId(), dto));
        }

        // Member: the caller's own requests
        [Authorize(Roles = "Member")]
        [HttpGet("mine")]
        public async Task<ActionResult<List<BookRequestDto>>> GetMine()
        {
            return await _service.GetMineAsync(User.GetUserId());
        }

        // Admin: all pending requests
        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<ActionResult<List<BookRequestDto>>> GetPending()
            => await _service.GetPendingAsync();

        // Admin: approve (creates the loan)
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
