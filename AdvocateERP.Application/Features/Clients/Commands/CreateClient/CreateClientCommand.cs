using MediatR;
using System;

namespace AdvocateERP.Application.Features.Clients.Commands.CreateClient
{
    public class CreateClientCommand : IRequest<Guid> // IRequest<Guid> is crucial
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }
}