using Microsoft.AspNetCore.Identity;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateJwtToken(IdentityUser<Guid> user, IList<string> roles);
}