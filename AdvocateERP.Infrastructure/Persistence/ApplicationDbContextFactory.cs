using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using AdvocateERP.Infrastructure.Services; // For TenantService dummy implementation
using AdvocateERP.Application.Interfaces.Services; // For ITenantService

namespace AdvocateERP.Infrastructure.Persistence
{
    // This factory is ONLY used by the dotnet ef commands (Add-Migration, Update-Database)
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 1. Setup the connection string (must be hardcoded or read from a configuration file here)
            var connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Database=AdvocateERPSaaS;Trusted_Connection=True;MultipleActiveResultSets=true";

            // 2. Setup the options builder
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString,
                // IMPORTANT: Point to the assembly where the DbContext resides
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            // 3. Create a DUMMY TenantService, as the tooling cannot inject HttpContext/Claims
            // This allows the DbContext to be instantiated without relying on the full Web DI stack.
            var dummyTenantService = new TenantService() as ITenantService;

            // 4. Instantiate and return the DbContext
            return new ApplicationDbContext(optionsBuilder.Options, dummyTenantService);
        }
    }
}