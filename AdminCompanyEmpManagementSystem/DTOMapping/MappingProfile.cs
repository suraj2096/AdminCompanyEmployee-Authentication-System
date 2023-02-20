using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Models.DTOs;
using AutoMapper;

namespace AdminCompanyEmpManagementSystem.DTOMapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegisterDTO, ApplicationUser>().ReverseMap();
            CreateMap<CompanyDTO, Company>().ReverseMap();
            CreateMap<EmployeeDTO, Employee>().ReverseMap();
        }
    }
}
