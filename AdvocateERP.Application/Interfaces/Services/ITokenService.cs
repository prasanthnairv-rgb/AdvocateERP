using AdvocateERP.Core.Entities;

namespace AdvocateERP.Application.Interfaces.Services;

public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}