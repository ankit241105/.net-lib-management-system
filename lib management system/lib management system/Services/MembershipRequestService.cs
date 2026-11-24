using lib_management_system.Common;
using lib_management_system.Dtos;
using lib_management_system.Models;
using lib_management_system.Repositories;

namespace lib_management_system.Services
{
    public class MembershipRequestService : IMembershipRequestService
    {
        // Mock membership price (₹). No real payment gateway is involved.
        private const decimal MembershipPrice = 199m;

        private readonly IMembershipRequestRepository _requestRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<MembershipRequestService> _logger;

        public MembershipRequestService(
            IMembershipRequestRepository requestRepo,
            IUserRepository userRepo,
            ILogger<MembershipRequestService> logger)
        {
            _requestRepo = requestRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<ServiceResult<MembershipRequestDto>> CreateAsync(int userId)
        {
            if (await _requestRepo.HasPendingAsync(userId))
                return ServiceResult<MembershipRequestDto>.Fail(400, "You already have a pending membership request.");

            var request = new MembershipRequest
            {
                UserId = userId,
                Amount = MembershipPrice,
                Status = "Pending",
                RequestedDate = DateTime.UtcNow
            };
            await _requestRepo.AddAsync(request);
            await _requestRepo.SaveChangesAsync();

            _logger.LogInformation("Membership request {RequestId} created by user {UserId}", request.Id, userId);
            return ServiceResult<MembershipRequestDto>.Ok(request.ToDto(), 201);
        }

        public async Task<List<MembershipRequestDto>> GetMineAsync(int userId)
        {
            var requests = await _requestRepo.GetByUserAsync(userId);
            return requests.Select(r => r.ToDto()).ToList();
        }

        public async Task<List<MembershipRequestDto>> GetPendingAsync()
        {
            var requests = await _requestRepo.GetPendingAsync();
            return requests.Select(r => r.ToDto()).ToList();
        }

        public async Task<ServiceResult<MembershipRequestDto>> ApproveAsync(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request is null)
                return ServiceResult<MembershipRequestDto>.Fail(404, "Request not found.");

            if (request.Status != "Pending")
                return ServiceResult<MembershipRequestDto>.Fail(400, "This request has already been decided.");

            var user = await _userRepo.GetByIdAsync(request.UserId);
            if (user is null)
                return ServiceResult<MembershipRequestDto>.Fail(404, "The requesting user no longer exists.");

            // Approving the upgrade flips the account's role. The user must log in again to
            // get a new token carrying the "Member" role.
            user.Role = "Member";
            request.Status = "Approved";
            request.DecisionDate = DateTime.UtcNow;
            await _requestRepo.SaveChangesAsync();

            _logger.LogInformation("Membership request {RequestId} approved: user {UserId} is now a Member",
                request.Id, request.UserId);

            return ServiceResult<MembershipRequestDto>.Ok(request.ToDto(), 200);
        }

        public async Task<ServiceResult<MembershipRequestDto>> DeclineAsync(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request is null)
                return ServiceResult<MembershipRequestDto>.Fail(404, "Request not found.");

            if (request.Status != "Pending")
                return ServiceResult<MembershipRequestDto>.Fail(400, "This request has already been decided.");

            request.Status = "Declined";
            request.DecisionDate = DateTime.UtcNow;
            await _requestRepo.SaveChangesAsync();

            _logger.LogInformation("Membership request {RequestId} declined", request.Id);
            return ServiceResult<MembershipRequestDto>.Ok(request.ToDto(), 200);
        }
    }
}
