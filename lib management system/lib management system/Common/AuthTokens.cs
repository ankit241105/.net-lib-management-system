namespace lib_management_system.Common
{
    // What a successful login / refresh produces: a short-lived access token plus the
    // raw refresh token (which the controller puts into an httpOnly cookie).
    public class AuthTokens
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime RefreshTokenExpires { get; init; }
    }
}
