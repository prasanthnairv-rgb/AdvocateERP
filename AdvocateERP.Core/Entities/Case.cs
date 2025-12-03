using AdvocateERP.Core.Common;
using AdvocateERP.Core.Enums;
using System;
using System.Collections.Generic;

namespace AdvocateERP.Core.Entities
{
    public class Case : BaseEntity, ITenantEntity
    {
        // 1. Multi-Tenancy Implementation
        public Guid TenantId { get; set; }

        public string CaseTitle { get; set; } = string.Empty;
        public string CaseNumber { get; set; } = string.Empty; // Court-assigned number
        public string CourtName { get; set; } = string.Empty; // e.g., High Court, District Court

        public CaseStatus Status { get; set; } = CaseStatus.Draft;
        public string Description { get; set; } = string.Empty;

        public DateTime FilingDate { get; set; }
        public DateTime? ClosureDate { get; set; }

        // 2. Foreign Key (Many-to-One): A Case belongs to one Client
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!; // Required Navigation Property

        // Note: We will define an Advocate entity later to link the assigned lawyer
    }
}