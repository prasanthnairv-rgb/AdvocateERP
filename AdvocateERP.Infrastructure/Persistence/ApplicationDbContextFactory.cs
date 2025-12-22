using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using AdvocateERP.Application.Interfaces.Services; // Keep this for ITenantService

namespace AdvocateERP.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Database=AdvocateERPSaaS;Trusted_Connection=True;MultipleActiveResultSets=true";
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // This now points to the class below, which takes NO arguments
            ITenantService dummyTenantService = new DesignTimeTenantService();

            return new ApplicationDbContext(optionsBuilder.Options, dummyTenantService);
        }
    }

    // This class is in the SAME namespace and file, so it doesn't need a 'using'
    internal class DesignTimeTenantService : ITenantService
    {
        public Guid? TenantId => Guid.Empty;
        public void SetTenantId(Guid tenantId) { }
    }
}