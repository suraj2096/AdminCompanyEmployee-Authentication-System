using AdminCompanyEmpManagementSystem.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdminCompanyEmpManagementSystem.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string GstNum { get; set; }
        [Required]
        public string Address { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
