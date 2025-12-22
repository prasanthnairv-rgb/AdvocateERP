using Microsoft.AspNetCore.Identity;
namespace AdvocateERP.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public Guid TenantId { get; set; }
    public string FullName { get; set; } = string.Empty;
}