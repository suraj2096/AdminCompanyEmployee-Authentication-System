using System.ComponentModel.DataAnnotations;

namespace AdminCompanyEmpManagementSystem.Models
{
    public class UserLoginDetail
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
