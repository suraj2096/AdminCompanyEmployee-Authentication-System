using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Repository.IRepository;

namespace AdminCompanyEmpManagementSystem.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;  
        public UnitOfWork(ApplicationDbContext context) {
            _context = context;
           
            _companyRepository= new CompanyRepository(_context);
            _employeeRepository= new EmployeeRepository(_context);
            _leaveRepository = new LeaveRepository(_context);
            _designationRepository = new DesignationRepository(_context);
            _allotedDesignationRepository = new AllotedDesignationRepository(_context);
        }
       

        public CompanyRepository _companyRepository { get; private set; }

        public EmployeeRepository _employeeRepository { get; private set; }

        public LeaveRepository _leaveRepository { get; private set; }
        public DesignationRepository _designationRepository { get; private set; }
        public AllotedDesignationRepository _allotedDesignationRepository { get; private set; }
    }
}
