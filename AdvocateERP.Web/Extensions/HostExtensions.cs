using AdvocateERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AdvocateERP.Web.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            // Use a scope to resolve services like DbContext
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

                try
                {
                    logger.LogInformation("Attempting to migrate database for ApplicationDbContext...");

                    var dbContext = services.GetRequiredService<ApplicationDbContext>();

                    // This is the key line: it applies any pending migrations (like InitialAdvocateSchema)
                    dbContext.Database.Migrate();

                    logger.LogInformation("Database migration complete.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database.");
                    // In a real application, you might stop the host here
                }
            }
            return host;
        }
    }
}