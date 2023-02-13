using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminCompanyEmpManagementSystem.Identity
{
    public class ApplicationUser:IdentityUser
    {
        [NotMapped]
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenValidDate { get; set; }
        [NotMapped]
        public string? Role { get; set; }
    }
}
