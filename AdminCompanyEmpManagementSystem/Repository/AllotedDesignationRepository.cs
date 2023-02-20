using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Repository.IRepository;

namespace AdminCompanyEmpManagementSystem.Repository
{
    public class AllotedDesignationRepository:Repository<AllotedDesignationEmployee>,IAllotedDesignationRepository
    {
        public AllotedDesignationRepository(ApplicationDbContext context):base(context) { }
    }
}
