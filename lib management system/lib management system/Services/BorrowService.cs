using lib_management_system.Common;
using lib_management_system.Dtos;
using lib_management_system.Models;
using lib_management_system.Repositories;

namespace lib_management_system.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepo;
        private readonly ILogger<BorrowService> _logger;

        public BorrowService(IBorrowRepository borrowRepo, ILogger<BorrowService> logger)
        {
            _borrowRepo = borrowRepo;
            _logger = logger;
        }

        public async Task<List<BorrowRecordDto>> GetRecordsAsync()
        {
            var records = await _borrowRepo.GetAllAsync();
            return records.Select(r => r.ToDto()).ToList();
        }

        public async Task<List<BorrowRecordDto>> GetMyRecordsAsync(int userId)
        {
            var records = await _borrowRepo.GetByUserAsync(userId);
            return records.Select(r => r.ToDto()).ToList();
        }

        public async Task<ServiceResult<BorrowRecordDto>> ReturnAsync(int id)
        {
            var record = await _borrowRepo.GetByIdWithBookAsync(id);
            if (record is null)
            {
                _logger.LogWarning("Return failed: borrow record {RecordId} not found", id);
                return ServiceResult<BorrowRecordDto>.Fail(404, "Borrow record not found.");
            }

            if (record.ReturnDate != null)
            {
                _logger.LogWarning("Return failed: record {RecordId} already returned", id);
                return ServiceResult<BorrowRecordDto>.Fail(400, "This book has already been returned.");
            }

            record.ReturnDate = DateTime.UtcNow;
            record.Fine = Fines.Calculate(record.DueDate, record.ReturnDate.Value);
            if (record.Book != null) record.Book.IsAvailable = true;
            await _borrowRepo.SaveChangesAsync();

            _logger.LogInformation("Book returned (record {RecordId}, book {BookId}, fine ₹{Fine})",
                record.Id, record.BookId, record.Fine);
            return ServiceResult<BorrowRecordDto>.Ok(record.ToDto(), 200);
        }
    }
}
