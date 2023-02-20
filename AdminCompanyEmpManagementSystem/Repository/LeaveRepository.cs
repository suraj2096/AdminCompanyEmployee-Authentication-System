using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Repository.IRepository;

namespace AdminCompanyEmpManagementSystem.Repository
{
    public class LeaveRepository:Repository<Leaves>,ILeaveRepository
    {
        public LeaveRepository(ApplicationDbContext context) : base(context) { }
    }
}
