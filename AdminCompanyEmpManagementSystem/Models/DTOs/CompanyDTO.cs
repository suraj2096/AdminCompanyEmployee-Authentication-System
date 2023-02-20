using System.ComponentModel.DataAnnotations;

namespace AdminCompanyEmpManagementSystem.Models.DTOs
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get ; set;  }
        [Required]
        public string Email { get; set; }
        [Required]
        public string GstNum { get; set; }
        [Required]
        public string Address { get; set; }
        public string ApplicationUserId { get; set; }
        public List<CompanyDesignationDTO>? CompanyDesigantion { get;set; }

      
    }
}
