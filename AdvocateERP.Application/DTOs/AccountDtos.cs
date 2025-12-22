namespace AdvocateERP.Application.DTOs;

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
}