using AdvocateERP.Core.Entities;
using MediatR;
using AdvocateERP.Application.Interfaces;

namespace AdvocateERP.Application.Features.Clients.Commands.CreateClient
{
    // IRequestHandler<The Command, The Return Type>
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Guid> 
    {
        private readonly IApplicationDbContext _context;

        public CreateClientCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            // Use Client entity from AdvocateERP.Core
            var client = new Client
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                ContactNumber = request.ContactNumber,
                AddressLine1 = request.AddressLine1,
                City = request.City,
                PostalCode = request.PostalCode
                // TenantId, Auditing handled by SaveChangesAsync
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync(cancellationToken);

            return client.Id;
        }
    }
}