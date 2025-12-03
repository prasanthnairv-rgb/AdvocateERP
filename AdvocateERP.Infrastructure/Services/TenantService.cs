using AdvocateERP.Application.Interfaces.Services;
using System;

// In AdvocateERP.Infrastructure/Services
namespace AdvocateERP.Infrastructure.Services
{
    // Registered as a Scoped service in Program.cs
    public class TenantService : ITenantService
    {
        // Use a nullable Guid to represent the TenantId
        public Guid? TenantId { get; private set; }

        public void SetTenantId(Guid tenantId)
        {
            TenantId = tenantId;
        }
    }
}