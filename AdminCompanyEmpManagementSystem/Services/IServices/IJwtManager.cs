using AdminCompanyEmpManagementSystem.Identity;
using System.Security.Claims;

namespace AdminCompanyEmpManagementSystem.Services.IServices
{
    public interface IJwtManager
    {
        ApplicationUser GenerateToken(ApplicationUser user, bool isGenerateRefreshToken);
        ClaimsPrincipal? GetClaimsFromExpiredToken(string token);
    }
}
