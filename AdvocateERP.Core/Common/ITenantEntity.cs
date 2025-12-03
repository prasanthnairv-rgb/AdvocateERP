using System;

namespace AdvocateERP.Core.Common
{
    // The contract ensuring every relevant entity has a Guid TenantId for multi-tenancy
    public interface ITenantEntity
    {
        Guid TenantId { get; set; }
    }
}