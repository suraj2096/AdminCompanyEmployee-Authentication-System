using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Repository.IRepository;

namespace AdminCompanyEmpManagementSystem.Repository
{
    public class DesignationRepository:Repository<Designation>,IDesignationRepository
    {
        public DesignationRepository(ApplicationDbContext context):base(context) { }
    }
}
