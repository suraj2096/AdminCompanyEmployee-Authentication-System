using System.ComponentModel.DataAnnotations.Schema;

namespace AdminCompanyEmpManagementSystem.Models
{
    public class AllotedDesignationEmployee
    {
        public int Id { get; set; }
        public int name { get; set; }
        public int DesignationId { get; set; }
        [ForeignKey("DesignationId")]
        public Designation Designation { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
