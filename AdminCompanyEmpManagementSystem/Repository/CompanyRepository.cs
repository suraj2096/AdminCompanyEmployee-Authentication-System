using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Repository.IRepository;
using Microsoft.AspNetCore.SignalR;

namespace AdminCompanyEmpManagementSystem.Repository
{
    public class CompanyRepository:Repository<Company>,ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext context):base(context)
        {

        }
    }
}
