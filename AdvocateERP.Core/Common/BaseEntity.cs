using System;

namespace AdvocateERP.Core.Common
{
    // A base class for common properties like ID and auditing metadata
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        // Tracking when the record was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; } // User ID or Name

        // Tracking the last modification
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; } // User ID or Name
    }
}