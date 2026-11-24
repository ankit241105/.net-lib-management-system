using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using lib_management_system.Common;
using lib_management_system.Dtos;
using lib_management_system.Models;
using lib_management_system.Repositories;

namespace lib_management_system.Services
{
    public class AuthService : IAuthService
    {
        private const int AccessTokenMinutes = 15;
        private const int RefreshTokenDays = 7;

        private readonly IUserRepository _users;
        private readonly IRefreshTokenRepository _refreshTokens;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository users,
            IRefreshTokenRepository refreshTokens,
            IConfiguration config,
            ILogger<AuthService> logger)
        {
            _users = users;
            _refreshTokens = refreshTokens;
            _config = config;
            _logger = logger;
        }

        // Public self-registration -> always a normal "User"
        public Task<ServiceResult<string>> RegisterAsync(RegisterRequest request)
            => CreateUserAsync(request, "User");

        // Admin-only -> creates another "Admin"
        public async Task<ServiceResult<string>> RegisterAdminAsync(RegisterRequest request, string? creator)
        {
            var result = await CreateUserAsync(request, "Admin");
            if (result.Success)
            {
                _logger.LogInformation("Admin {Creator} created a new admin account {NewAdmin}",
                    creator, request.Username);
            }
            return result;
        }

        public async Task<ServiceResult<AuthTokens>> LoginAsync(LoginRequest request)
        {
            var user = await _users.GetByUsernameAsync(request.Username);
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for username {Username}", request.Username);
                return ServiceResult<AuthTokens>.Fail(401, "Invalid username or password.");
            }

            var tokens = await IssueTokensAsync(user);
            _logger.LogInformation("User {Username} logged in successfully", user.Username);
            return ServiceResult<AuthTokens>.Ok(tokens);
        }

        public async Task<ServiceResult<AuthTokens>> RefreshAsync(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return ServiceResult<AuthTokens>.Fail(401, "No refresh token.");

            var existing = await _refreshTokens.GetByTokenAsync(refreshToken);
            if (existing is null || !existing.IsActive || existing.User is null)
            {
                _logger.LogWarning("Refresh rejected: token missing, expired or revoked");
                return ServiceResult<AuthTokens>.Fail(401, "Invalid or expired refresh token.");
            }

            // Rotate: revoke the used token and issue a fresh pair. The new access token
            // reflects the user's CURRENT role (so a role change takes effect on next refresh).
            existing.RevokedAt = DateTime.UtcNow;
            var tokens = await IssueTokensAsync(existing.User);
            _logger.LogInformation("Refresh token rotated for user {Username}", existing.User.Username);
            return ServiceResult<AuthTokens>.Ok(tokens);
        }

        public async Task LogoutAsync(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return;
            var existing = await _refreshTokens.GetByTokenAsync(refreshToken);
            if (existing is not null && existing.RevokedAt is null)
            {
                existing.RevokedAt = DateTime.UtcNow;
                await _refreshTokens.SaveChangesAsync();
                _logger.LogInformation("Refresh token revoked on logout for user {UserId}", existing.UserId);
            }
        }

        // Shared helper: the role is ALWAYS decided here on the server, never taken from the request
        private async Task<ServiceResult<string>> CreateUserAsync(RegisterRequest request, string role)
        {
            if (await _users.ExistsByUsernameAsync(request.Username))
                return ServiceResult<string>.Fail(400, "Username already taken.");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = role
            };
            await _users.AddAsync(user);
            await _users.SaveChangesAsync();

            _logger.LogInformation("New {Role} account '{Username}' created", role, request.Username);
            return ServiceResult<string>.Ok($"{role} registered successfully.");
        }

        // Builds + persists a new access/refresh pair for a user.
        private async Task<AuthTokens> IssueTokensAsync(User user)
        {
            var refresh = new RefreshToken
            {
                Token = GenerateRefreshTokenString(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDays)
            };
            await _refreshTokens.AddAsync(refresh);
            await _refreshTokens.SaveChangesAsync();

            return new AuthTokens
            {
                AccessToken = GenerateAccessToken(user),
                RefreshToken = refresh.Token,
                RefreshTokenExpires = refresh.ExpiresAt
            };
        }

        private static string GenerateRefreshTokenString() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
