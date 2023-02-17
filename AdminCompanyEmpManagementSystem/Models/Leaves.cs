namespace AdminCompanyEmpManagementSystem.Models
{
    public class Leaves
    {
        public Leaves()
        {
            TotalLeave = 10;
        }
        public int Id { get; set; } 
        public DateTime LeaveDate { get; set; }
        public int TotalLeave { get; set; }
        public string LeaveStatus { get; set; }
        public int EmpId { get; set; }
        public Employee Employee { get; set; }
    }
}
