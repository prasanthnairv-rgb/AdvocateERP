using AdvocateERP.Application.DTOs;
using AdvocateERP.Application.Interfaces.Services;
using AdvocateERP.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc; // Essential for ControllerBase and HttpPost
using System.Threading.Tasks;

namespace AdvocateERP.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    // Inject UserManager and your TokenService
    public AccountController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) // LoginDto and UserDto defined in Application/DTOs
    {
        var user = await _userManager.FindByNameAsync(loginDto.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return Unauthorized("Invalid credentials");

        return new UserDto
        {
            Username = user.UserName!,
            Token = _tokenService.CreateToken(user),
            TenantId = user.TenantId
        };
    }
}