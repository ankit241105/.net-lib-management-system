namespace lib_management_system.Dtos
{
    // Request: public self-registration or admin-created account.
    // Defaults keep an omitted field as "" (e.g. register-admin sends no email).
    public record RegisterRequest(string Username = "", string Email = "", string Password = "");

    // Request: username + password login.
    public record LoginRequest(string Username = "", string Password = "");
}
