using Microsoft.Extensions.Hosting;
using AdvocateERP.Application.Interfaces;
using AdvocateERP.Application.Interfaces.Services;
using AdvocateERP.Infrastructure.Persistence;
using AdvocateERP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using AdvocateERP.Web.Extensions;
// Add the using directive for MediatR configuration
using MediatR;
using AdvocateERP.Application; // Use a type from the Application assembly

var builder = WebApplication.CreateBuilder(args);


// FORCE ENVIRONMENT TO DEVELOPMENT TO BYPASS EXTERNAL OVERRIDE
builder.Environment.EnvironmentName = Environments.Development;

// Add services to the container.

// --- Register MediatR and Application Layer ---
// This scans the assembly where IRequestHandler is defined (AdvocateERP.Application) 
// to register all command and query handlers.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationDbContext).Assembly));

// --- 1. Register Infrastructure Services (Tenant Service) ---
// Register ITenantService as SCOPED so a new instance exists for every HTTP request, 
// allowing the TenantId to be unique per client request.
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);
// --- 2. Register ApplicationDbContext (SQL Server) ---
// We use the DefaultConnection string defined in appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString,
        // Specify the assembly where migrations are stored (Infrastructure)
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));


// --- Other Web Services ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ... after app.Build()

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Ensure Swagger is enabled FIRST in the pipeline
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Explicitly defines the Swagger JSON endpoint.
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdvocateERP API V1");

        // This makes Swagger UI the default page when you navigate to the base URL
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// UseRouting is implicitly called by MapControllers but explicitly calling it here can help resolve routing issues.
app.UseRouting();

// --- 3. Register Tenant Identification Middleware ---
// This should run AFTER routing and authentication, but before controllers execute.
app.Use(async (context, next) =>
{
    // ... your dummy tenant ID setup ...
    var tenantService = context.RequestServices.GetRequiredService<ITenantService>();
    tenantService.SetTenantId(new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"));
    await next(context);
});

app.UseAuthorization(); // Authorization runs after routing and custom logic

// MapControllers runs the endpoints.
app.MapControllers();

app.Run();