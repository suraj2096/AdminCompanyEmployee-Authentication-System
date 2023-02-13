using System.ComponentModel.DataAnnotations;

namespace AdminCompanyEmpManagementSystem.Models
{
    public class UserToken
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
