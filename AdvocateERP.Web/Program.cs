using Microsoft.Extensions.Hosting;
using AdvocateERP.Application.Interfaces;
using AdvocateERP.Application.Interfaces.Services;
using AdvocateERP.Infrastructure.Persistence;
using AdvocateERP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using AdvocateERP.Web.Extensions;
using MediatR;
using AdvocateERP.Application;
using Microsoft.AspNetCore.Identity;
using AdvocateERP.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddDataProtection();

// FORCE ENVIRONMENT TO DEVELOPMENT
builder.Environment.EnvironmentName = Environments.Development;

// --- 1. CORS Policy Registration (Must be before builder.Build) ---
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// --- 2. Identity Services ---
builder.Services.AddIdentityCore<ApplicationUser>(opt => {
    opt.Password.RequireNonAlphanumeric = false; // Easier for testing
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddSignInManager<SignInManager<ApplicationUser>>();

// --- 3. Database & MediatR ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString,
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationDbContext).Assembly));

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. SEED DATA LOGIC (Synchronous Force) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    try
    {

        Console.WriteLine("--> [DEBUG] Applying pending migrations...");
        // THIS LINE PHYSICALLY CREATES THE TABLES IF THEY ARE MISSING
        context.Database.Migrate();

        Console.WriteLine("--> [DEBUG] Checking Tenants...");
        var tenantId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");

        if (!context.Tenants.Any())
        {
            context.Tenants.Add(new Tenant { Id = tenantId, Name = "Admin Law Firm" });
            context.SaveChanges();
            Console.WriteLine("--> [DEBUG] Tenant Created.");
        }

        Console.WriteLine("--> [DEBUG] Checking Users...");
        // Use .Result to force the async call to finish immediately
        var existingUser = userManager.FindByEmailAsync("admin@test.com").Result;

        if (existingUser == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                FullName = "System Admin",
                TenantId = tenantId,
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(adminUser, "Pa$$w0rd123!").Result;

            if (result.Succeeded)
            {
                Console.WriteLine("--> [DEBUG] SUCCESS: admin@test.com created.");
            }
            else
            {
                Console.WriteLine("--> [DEBUG] FAILED: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            Console.WriteLine("--> [DEBUG] User already exists.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("--> [DEBUG] CRITICAL ERROR: " + ex.Message);
        if (ex.InnerException != null)
            Console.WriteLine("--> [DEBUG] INNER: " + ex.InnerException.Message);
    }
}

// --- 5. Middleware Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdvocateERP API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins); // Must be after UseRouting

// Custom Tenant Middleware
app.Use(async (context, next) =>
{
    var tenantService = context.RequestServices.GetRequiredService<ITenantService>();
    tenantService.SetTenantId(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"));
    await next(context);
});

app.UseAuthentication(); // ADDED THIS
app.UseAuthorization();

app.MapControllers();
app.Run();