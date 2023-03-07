using System.ComponentModel.DataAnnotations.Schema;

namespace AdminCompanyEmpManagementSystem.Models
{
    public class Leaves
    {
       
        public int Id { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
       
        public enum LeaveStatus
        {
            Approved=1,
            Pending=2,
            Rejected=3
        };
        public LeaveStatus Status { get; set; }
        public int EmpId { get; set; }
        [ForeignKey("EmpId")]
        public Employee Employee { get; set; }
    }
}
