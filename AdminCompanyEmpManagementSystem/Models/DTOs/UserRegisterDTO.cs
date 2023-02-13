using System.ComponentModel.DataAnnotations;

namespace AdminCompanyEmpManagementSystem.Models.DTOs
{
    public class UserRegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
