using System.ComponentModel.DataAnnotations;

namespace AdminCompanyEmpManagementSystem.Models
{
    public class Designation
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
