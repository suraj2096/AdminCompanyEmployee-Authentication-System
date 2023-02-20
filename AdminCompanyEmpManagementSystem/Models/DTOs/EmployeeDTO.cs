using AdminCompanyEmpManagementSystem.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminCompanyEmpManagementSystem.Models.DTOs
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDTO : ControllerBase
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
    }
}
