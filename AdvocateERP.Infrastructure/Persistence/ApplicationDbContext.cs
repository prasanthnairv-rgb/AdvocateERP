using AdvocateERP.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Required for Identity
using Microsoft.EntityFrameworkCore;
using AdvocateERP.Application.Interfaces;
using AdvocateERP.Application.Interfaces.Services;
using AdvocateERP.Core.Common;
using AdvocateERP.Infrastructure.Services;
using System.Linq.Expressions;

namespace AdvocateERP.Infrastructure.Persistence
{
    // FIX 1: Inherit from IdentityDbContext<ApplicationUser> instead of DbContext
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        private readonly ITenantService _tenantService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ITenantService tenantService)
            : base(options)
        {
            _tenantService = tenantService;
        }

        // FIX 2: Add the Tenants DbSet so Program.cs can see it
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Case> Cases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // FIX 3: Always call base.OnModelCreating first for Identity tables
            base.OnModelCreating(modelBuilder);

            var currentTenantId = _tenantService.TenantId;

            // Apply Global Filter to all entities implementing ITenantEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var propertyAccess = Expression.Property(parameter, nameof(ITenantEntity.TenantId));
                    var constantValue = Expression.Constant(currentTenantId, typeof(Guid?));
                    var nullablePropertyAccess = Expression.Convert(propertyAccess, typeof(Guid?));
                    var filter = Expression.Equal(nullablePropertyAccess, constantValue);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(filter, parameter));
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentTenantId = _tenantService.TenantId;
            var currentUserId = "System";

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.Entity is ITenantEntity tenantEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        if (!currentTenantId.HasValue)
                        {
                            // During migrations, we might not have a tenant, 
                            // but for real app usage, this is a safety guard.
                            tenantEntity.TenantId = Guid.Empty;
                        }
                        else
                        {
                            tenantEntity.TenantId = currentTenantId.Value;
                        }

                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = currentUserId;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = currentUserId;
                        entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                        entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                        entry.Property(nameof(ITenantEntity.TenantId)).IsModified = false;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}