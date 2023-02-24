using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Models.DTOs;
using AdminCompanyEmpManagementSystem.Repository.IRepository;
using AdminCompanyEmpManagementSystem.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminCompanyEmpManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public ManagementController(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
        }
        #region Company Related work Here
        [Route("/Admin/Companies")]
        [HttpGet]
        public IActionResult GetAllCompnay()
        {
            // return Ok(_unitOfWork._companyRepository.GetAll());
            List<CompanyDTO> companyDesigantionDto = new List<CompanyDTO>();
            var getAllCompany = _unitOfWork._companyRepository.GetAll();
           
            for(int i=0;i<getAllCompany.Count;i++)
            {
                CompanyDTO company = new CompanyDTO()
                {
                    CompanyDesigantion = new List<CompanyDesignationDTO>()
                };
                var getAllDesEmp = _unitOfWork._allotedDesignationRepository.GetAll(u => u.CompanyId == getAllCompany.ElementAt(i).Id, includeTables: "Designation");
                company.Id = getAllCompany.ElementAt(i).Id;
                company.Name = getAllCompany.ElementAt(i).Name;
                company.Address = getAllCompany.ElementAt(i).Address;
                company.GstNum = getAllCompany.ElementAt(i).GstNum;
                company.ApplicationUserId = getAllCompany.ElementAt(i).ApplicationUserId;
                var companyDesigantion = new List<CompanyDesignationDTO>();
                foreach(var dataDesigantion in getAllDesEmp)
                {
                    var companyDesigantionDTO = new  CompanyDesignationDTO()
                    {
                        Name = dataDesigantion.Name,
                        DesignationType = dataDesigantion.Designation.Name
                    };
                    companyDesigantion.Add(companyDesigantionDTO);
                }
                company.CompanyDesigantion.AddRange(companyDesigantion);
                companyDesigantionDto.Add(company);
            }
            return Ok(companyDesigantionDto);
        }



        // Summmary:
        /* In the Below Route /CreateCompany Here admin will create company and company will also create himself.
         and here we will create designation dynamically.
         */
        [Route("/CreateCompany")]
        [HttpPost]

        public async Task<IActionResult> CreateCompany([FromBody] CompanyDTO companyDTO)
        {
            //Patch:
            // Here some changes will done for application user so we will modify it later 
            if (companyDTO == null || !ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var companyDetail = _mapper.Map<Company>(companyDTO);
            // check if admin register the company then we first create the company register and login ceredentials
            ApplicationUser user = new ApplicationUser()
            {
                UserName = companyDTO.Email,
                PasswordHash = _userService.GeneratePassword(),
                Role = SD.Role_Company
            };
            var registerCompany = await _userService.RegisterUser(user);
            companyDetail.ApplicationUserId = user.Id;
            if (!_unitOfWork._companyRepository.Add(companyDetail))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //---------------------
            // this code is not required
           /* // create company here .
            // and also check that if company register in asp.net user then we will here update his details by 
            // company user.
            _unitOfWork._companyRepository.Add(companyDetail);*/
           //------------------------------
            for (int i = 0; i < companyDTO.CompanyDesigantion?.Count(); i++)
            {
                var designationExist = _unitOfWork._designationRepository.FirstOrDefault(filter: u => u.Name == companyDTO.CompanyDesigantion.ElementAt(i).DesignationType);
                var designationId = 0;
                if (designationExist == null)
                {
                    Designation designation = new Designation()
                    {
                        Name = companyDTO.CompanyDesigantion.ElementAt(i).DesignationType?.Trim().ToString() ?? "",
                    };
                    var createDesignation = _unitOfWork._designationRepository.Add(designation);
                    if (createDesignation)
                    {
                        designationId = _unitOfWork._designationRepository.FirstOrDefault(u => u.Name == designation.Name)?.Id ?? 0;
                    }
                }
                // alloted the desigantion to the company senior employee.
                AllotedDesignationEmployee allotedDesigEmployee = new AllotedDesignationEmployee()
                {
                    Name = companyDTO.CompanyDesigantion.ElementAt(i).Name?.Trim().ToString() ?? "",
                    CompanyId = companyDetail.Id, // static company id gave here we will work it later on.
                    DesignationId = designationId == 0 ? designationExist?.Id ?? 0 : designationId
                };
                _unitOfWork._allotedDesignationRepository.Add(allotedDesigEmployee);
            }
            return Ok(new { Success = 1, Message = "Company Created Successfuly" });
        }


        //Summary:
        // Here we will update the company by both Admin and Company
        [Route("/UpdateCompany")]
        [HttpPatch]
        public IActionResult UpdateCompany([FromBody] CompanyDTO companyDTO)
        {
            if (companyDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var companyDetail = _mapper.Map<Company>(companyDTO);
            // check if admin create company
            // here check if we want to only update company and also its designation person.
            _unitOfWork._companyRepository.Update(companyDetail);
            if (companyDTO.CompanyDesigantion != null)
            {
                for (int i = 0; i < companyDTO.CompanyDesigantion?.Count(); i++)
                {
                    var designationExist = _unitOfWork._designationRepository.FirstOrDefault(filter: u => u.Name == companyDTO.CompanyDesigantion.ElementAt(i).DesignationType);
                    var designationId = 0;
                    bool CreateNewDesignation = false;
                    if (designationExist == null)
                    {
                        CreateNewDesignation = true;
                        Designation designation = new Designation()
                        {
                            Name = companyDTO.CompanyDesigantion.ElementAt(i).DesignationType?.Trim().ToString() ?? "",
                        };
                        var createDesignation = _unitOfWork._designationRepository.Add(designation);
                        if (createDesignation)
                        {
                            designationId = _unitOfWork._designationRepository.FirstOrDefault(u => u.Name == designation.Name)?.Id ?? 0;
                        }

                    }
                    // alloted the desigantion to the company senior employee.
                    AllotedDesignationEmployee allotedDesigEmployee = new AllotedDesignationEmployee()
                    {
                        Name = companyDTO.CompanyDesigantion.ElementAt(i).Name?.Trim().ToString() ?? "",
                        CompanyId = companyDetail.Id, // static company id gave here we will work it later on.
                        DesignationId = designationId == 0 ? designationExist?.Id ?? 0 : designationId
                    };
                    if (CreateNewDesignation)
                    {
                        _unitOfWork._allotedDesignationRepository.Add(allotedDesigEmployee);
                        
                    }
                    else
                    {
                    _unitOfWork._allotedDesignationRepository.Update(allotedDesigEmployee);

                    }
                }
            }
            return Ok(new { Status = 1, Message = "Updated Successfully" });
        }



        //Summary:
        // here we will delete the company
        [Route("/DeleteCompany/{CmpId}")]
        [HttpDelete]
        public IActionResult DeleteCompany(int CmpId)
        {
            if (CmpId == 0) return BadRequest();
           
            // find the company
            var companyExist = _unitOfWork._companyRepository.FirstOrDefault(u=>u.Id== CmpId);
            if (companyExist == null) return NotFound();
            if (!_unitOfWork._companyRepository.Delete(companyExist))
            {
            return StatusCode(StatusCodes.Status500InternalServerError);
            }
            // find the employee designation of that company and delete it from the database
            var getEmpAllotedDesignation = _unitOfWork._allotedDesignationRepository.GetAll(u=>u.CompanyId == CmpId);
            if (getEmpAllotedDesignation != null)
            {
                _unitOfWork._allotedDesignationRepository.RemoveRange(getEmpAllotedDesignation);
            }
            return Ok(new { Status = 1, Message = "Deleted Successfully" });
        }

        // Summary:
        // Get Company all employee
        [Route("Company/Employee/{CmpId}")]
        [HttpGet]
        public IActionResult GetCompanyEmployee(int CmpId)
        {
            if(CmpId == 0) return BadRequest();
            var EmpExistCompany = _unitOfWork._employeeRepository.GetAll(u => u.CompanyId == CmpId);
            if (EmpExistCompany == null) return NotFound();
            return Ok(new { Status = 1, Data = EmpExistCompany });
        }







        #endregion

        





    }

}
