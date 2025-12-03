using System;

namespace AdvocateERP.Application.DTOs.Clients
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}