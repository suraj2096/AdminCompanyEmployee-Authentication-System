using AdminCompanyEmpManagementSystem.Models;

namespace AdminCompanyEmpManagementSystem.Repository.IRepository
{
    public interface IUnitOfWork
    {
       
        public CompanyRepository _companyRepository { get;}
        public EmployeeRepository _employeeRepository { get; }
        public LeaveRepository _leaveRepository { get; }
        public DesignationRepository  _designationRepository { get; }
        public AllotedDesignationRepository _allotedDesignationRepository { get; }


    }
}
