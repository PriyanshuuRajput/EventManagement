using Domains.Entities;
using System.Security.Claims;

namespace Applications.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(AdminUser admin);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
