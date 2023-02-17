using AdminCompanyEmpManagementSystem.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdminCompanyEmpManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public int Salary { get; set; }
        [Required]
        public string Address { get; set; }
        public string PanNum { get; set; }
        [Required]
        public string AccountNum { get; set; }
        [Required]
        public string PFNum { get; set; }
        [Required]
        public string PhoneNum { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
