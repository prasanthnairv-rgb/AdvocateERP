using AdvocateERP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AdvocateERP.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        // Define all DbSets used by the Application layer
        DbSet<Client> Clients { get; set; }
        DbSet<Case> Cases { get; set; }
        // ... other DbSets

        // Define SaveChangesAsync method
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}