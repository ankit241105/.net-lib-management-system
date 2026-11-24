using lib_management_system.Common;
using lib_management_system.Dtos;
using lib_management_system.Models;
using lib_management_system.Repositories;

namespace lib_management_system.Services
{
    public class BookRequestService : IBookRequestService
    {
        private readonly IBookRequestRepository _requestRepo;
        private readonly IBookRepository _bookRepo;
        private readonly IBorrowRepository _borrowRepo;
        private readonly ILogger<BookRequestService> _logger;

        public BookRequestService(
            IBookRequestRepository requestRepo,
            IBookRepository bookRepo,
            IBorrowRepository borrowRepo,
            ILogger<BookRequestService> logger)
        {
            _requestRepo = requestRepo;
            _bookRepo = bookRepo;
            _borrowRepo = borrowRepo;
            _logger = logger;
        }

        public async Task<ServiceResult<BookRequestDto>> CreateAsync(int userId, CreateBookRequestDto dto)
        {
            var book = await _bookRepo.GetByIdAsync(dto.BookId);
            if (book is null)
                return ServiceResult<BookRequestDto>.Fail(404, "Book not found.");

            if (await _requestRepo.HasPendingForBookAsync(userId, dto.BookId))
                return ServiceResult<BookRequestDto>.Fail(400, "You already have a pending request for this book.");

            var request = new BookRequest
            {
                UserId = userId,
                BookId = dto.BookId,
                Status = "Pending",
                RequestedDate = DateTime.UtcNow
            };
            await _requestRepo.AddAsync(request);
            await _requestRepo.SaveChangesAsync();

            _logger.LogInformation("Book request {RequestId}: user {UserId} requested book {BookId}",
                request.Id, userId, dto.BookId);

            request.Book = book;
            return ServiceResult<BookRequestDto>.Ok(request.ToDto(), 201);
        }

        public async Task<List<BookRequestDto>> GetMineAsync(int userId)
        {
            var requests = await _requestRepo.GetByUserAsync(userId);
            return requests.Select(r => r.ToDto()).ToList();
        }

        public async Task<List<BookRequestDto>> GetPendingAsync()
        {
            var requests = await _requestRepo.GetPendingAsync();
            return requests.Select(r => r.ToDto()).ToList();
        }

        public async Task<ServiceResult<BookRequestDto>> ApproveAsync(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request is null)
                return ServiceResult<BookRequestDto>.Fail(404, "Request not found.");

            if (request.Status != "Pending")
                return ServiceResult<BookRequestDto>.Fail(400, "This request has already been decided.");

            if (request.Book is null)
                return ServiceResult<BookRequestDto>.Fail(404, "The requested book no longer exists.");

            if (!request.Book.IsAvailable)
                return ServiceResult<BookRequestDto>.Fail(400, "This book is no longer available.");

            // Approving a request IS the loan: create the borrow record and mark the book out.
            request.Book.IsAvailable = false;
            var borrowDate = DateTime.UtcNow;
            await _borrowRepo.AddAsync(new BorrowRecord
            {
                BookId = request.BookId,
                UserId = request.UserId,
                BorrowDate = borrowDate,
                DueDate = borrowDate.AddDays(Fines.LoanDays),
                ReturnDate = null
            });

            request.Status = "Approved";
            request.DecisionDate = DateTime.UtcNow;
            await _requestRepo.SaveChangesAsync();   // one save persists the record + book + request together

            _logger.LogInformation("Book request {RequestId} approved: book {BookId} loaned to user {UserId}",
                request.Id, request.BookId, request.UserId);

            return ServiceResult<BookRequestDto>.Ok(request.ToDto(), 200);
        }

        public async Task<ServiceResult<BookRequestDto>> DeclineAsync(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request is null)
                return ServiceResult<BookRequestDto>.Fail(404, "Request not found.");

            if (request.Status != "Pending")
                return ServiceResult<BookRequestDto>.Fail(400, "This request has already been decided.");

            request.Status = "Declined";
            request.DecisionDate = DateTime.UtcNow;
            await _requestRepo.SaveChangesAsync();

            _logger.LogInformation("Book request {RequestId} declined", request.Id);
            return ServiceResult<BookRequestDto>.Ok(request.ToDto(), 200);
        }
    }
}
