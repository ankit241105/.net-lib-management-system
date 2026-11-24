using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using lib_management_system.Dtos;
using lib_management_system.Extensions;
using lib_management_system.Services;

namespace lib_management_system.Controllers
{
    [Authorize]
    public class BorrowRecordsController : ApiControllerBase
    {
        private readonly IBorrowService _service;
        public BorrowRecordsController(IBorrowService service) => _service = service;

        // Admin: every loan (for the returns view)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<BorrowRecordDto>>> GetBorrowRecords()
            => await _service.GetRecordsAsync();

        // Member: only the caller's own loans
        [Authorize(Roles = "Member")]
        [HttpGet("mine")]
        public async Task<ActionResult<List<BorrowRecordDto>>> GetMyBorrowRecords()
        {
            return await _service.GetMyRecordsAsync(User.GetUserId());
        }

        // Admin: mark a book returned
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            return ToResponse(await _service.ReturnAsync(id));
        }
    }
}
