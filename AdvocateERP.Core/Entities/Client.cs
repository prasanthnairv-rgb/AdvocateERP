using AdvocateERP.Core.Common;
using System;
using System.Collections.Generic;

namespace AdvocateERP.Core.Entities
{
    public class Client : BaseEntity, ITenantEntity
    {
        // 1. Multi-Tenancy Implementation
        public Guid TenantId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        // Calculated property, not mapped to DB
        public string FullName => $"{FirstName} {LastName}";

        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        // 2. Navigation Property (One-to-Many): A Client can have many Cases
        public ICollection<Case> Cases { get; set; } = new List<Case>();
    }
}