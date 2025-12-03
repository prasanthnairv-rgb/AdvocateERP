using System;
// In AdvocateERP.Application/Interfaces/Services
namespace AdvocateERP.Application.Interfaces.Services
{
    public interface ITenantService
    {
        Guid? TenantId { get; }
        void SetTenantId(Guid tenantId);
    }
}