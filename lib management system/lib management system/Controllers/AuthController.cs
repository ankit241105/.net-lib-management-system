using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using lib_management_system.Dtos;
using lib_management_system.Services;

namespace lib_management_system.Controllers
{
    public class AuthController : ApiControllerBase
    {
        private const string RefreshCookie = "refreshToken";

        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        // POST: api/auth/register  -> public self-registration (always a "User")
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _auth.RegisterAsync(request);
            return ToResponse(result);
        }

        // POST: api/auth/register-admin  -> only an existing Admin can create another Admin
        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(RegisterRequest request)
        {
            var result = await _auth.RegisterAdminAsync(request, User.Identity?.Name);
            return ToResponse(result);
        }

        // POST: api/auth/login  -> returns the access token in the body, refresh token in an httpOnly cookie
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _auth.LoginAsync(request);
            if (!result.Success) return StatusCode(result.StatusCode, result.Error);

            SetRefreshCookie(result.Data!.RefreshToken, result.Data.RefreshTokenExpires);
            return Ok(new { token = result.Data.AccessToken });
        }

        // POST: api/auth/refresh  -> uses the refresh cookie to mint a new access token (and rotates the cookie)
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var cookie = Request.Cookies[RefreshCookie];
            var result = await _auth.RefreshAsync(cookie);
            if (!result.Success)
            {
                ClearRefreshCookie();
                return StatusCode(result.StatusCode, result.Error);
            }

            SetRefreshCookie(result.Data!.RefreshToken, result.Data.RefreshTokenExpires);
            return Ok(new { token = result.Data.AccessToken });
        }

        // POST: api/auth/logout  -> revokes the current refresh token and clears the cookie
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _auth.LogoutAsync(Request.Cookies[RefreshCookie]);
            ClearRefreshCookie();
            return Ok();
        }

        // --- cookie helpers ---
        // On localhost the app and the SPAs are same-site, so SameSite=Lax works over http.
        // In production (cross-site over HTTPS) use SameSite=None with Secure=true.
        private void SetRefreshCookie(string token, DateTime expires) =>
            Response.Cookies.Append(RefreshCookie, token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = false,
                Expires = expires,
                Path = "/api/auth"
            });

        private void ClearRefreshCookie() =>
            Response.Cookies.Delete(RefreshCookie, new CookieOptions { Path = "/api/auth" });
    }
}
