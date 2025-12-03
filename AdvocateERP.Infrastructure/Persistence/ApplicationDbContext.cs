using AdvocateERP.Application.Interfaces;
using AdvocateERP.Application.Interfaces.Services;
using AdvocateERP.Core.Common;
using AdvocateERP.Core.Entities;
using AdvocateERP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// In AdvocateERP.Infrastructure/Persistence
namespace AdvocateERP.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ITenantService _tenantService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ITenantService tenantService)
            : base(options)
        {
            _tenantService = tenantService;
        }

        // DbSets for your entities
        public DbSet<Client> Clients { get; set; }
        public DbSet<Case> Cases { get; set; }

        // We will add ASP.NET Core Identity later, which inherits from IdentityDbContext

        // --- Core Overrides for Tenancy and Auditing ---

        // 3A. Apply Multi-Tenancy Query Filter

        // --- In AdvocateERP.Infrastructure/Persistence/ApplicationDbContext.cs ---

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var currentTenantId = _tenantService.TenantId; // This is Guid? (Nullable)

            // Apply Global Filter to all entities implementing ITenantEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // 1. Get the expression parameter (e)
                    var parameter = Expression.Parameter(entityType.ClrType, "e");

                    // 2. Access the entity's TenantId property (e.TenantId)
                    // This property is a non-nullable Guid based on your interface.
                    var propertyAccess = Expression.Property(parameter, nameof(ITenantEntity.TenantId));

                    // 3. Define the constant value for the current TenantId
                    // The value is the non-nullable Guid from the service, but the type is Guid?
                    var constantValue = Expression.Constant(currentTenantId, typeof(Guid?));

                    // 4. FIX: Cast the entity's non-nullable TenantId property access to a nullable Guid?
                    // This makes the comparison (Guid? == Guid?) valid for EF Core translation.
                    // We use the Convert Expression to handle the cast.
                    var nullablePropertyAccess = Expression.Convert(propertyAccess, typeof(Guid?));

                    // 5. Build the filter: (Guid?)e.TenantId == _tenantService.TenantId
                    var filter = Expression.Equal(nullablePropertyAccess, constantValue);

                    // 6. Apply the filter
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(filter, parameter));
                }
            }
        }

        // 3B. Implement Auditing and Multi-Tenancy on Save
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentTenantId = _tenantService.TenantId;

            // Optional: If you need the current user ID for CreatedBy/ModifiedBy (not implemented yet)
            var currentUserId = "System"; // Replace with logic to get user ID from HttpContext/Claims

            // Critical loop to apply auditing and tenancy
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.Entity is ITenantEntity tenantEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        // 1. Mandatory Tenant ID Injection
                        if (!currentTenantId.HasValue)
                        {
                            throw new InvalidOperationException("Cannot save a new tenant entity without a valid Tenant ID context.");
                        }
                        tenantEntity.TenantId = currentTenantId.Value;

                        // 2. Auditing (Created)
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = currentUserId;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        // 3. Auditing (Modified)
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = currentUserId;

                        // Prevent changing CreatedBy/CreatedAt
                        entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                        entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;

                        // Prevent TenantId modification (critical security)
                        entry.Property(nameof(ITenantEntity.TenantId)).IsModified = false;
                    }
                }
                // Handle non-tenant BaseEntities if you had any
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}