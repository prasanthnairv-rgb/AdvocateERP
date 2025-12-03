using AdvocateERP.Application.DTOs.Clients;
using AdvocateERP.Application.Features.Clients.Commands.CreateClient; // NEW USING
using AdvocateERP.Infrastructure.Persistence;
using MediatR; // NEW USING
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvocateERP.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        // Add IMediator to the constructor
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context, IMediator mediator) // ADD MEDIATOR
        {
            _context = context;
            _mediator = mediator; // Initialize IMediator
        }

        // GET: api/Clients (Existing endpoint for verification)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            // The database call to fetch data (assuming _context.Clients exists)
            var clients = await _context.Clients
                .Select(c => new ClientDto
                {
                    // ... property assignments ...
                })
                .ToListAsync(); // <-- Ensure ToListAsync() is used

            if (!clients.Any())
            {
                // Path 1: Returns NotFound and exits the method.
                return NotFound("No clients found for this tenant.");
            }

            // Path 2: Returns Ok with data (This must be hit if the 'if' condition is false)
            return Ok(clients);

            // **CRITICAL:** Do NOT put 'return Ok(clients);' outside the method's curly braces.
        }

        // POST: api/Clients
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand command)
        {
            // Use MediatR to send the command to the appropriate handler
            Guid newClientId = await _mediator.Send(command);

            // Returns HTTP 201 Created status with the URI of the new resource
            return CreatedAtAction(nameof(GetClients), new { id = newClientId }, newClientId);
        }
    }
}