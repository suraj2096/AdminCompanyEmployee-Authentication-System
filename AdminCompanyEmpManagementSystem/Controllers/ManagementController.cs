using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Models.DTOs;
using AdminCompanyEmpManagementSystem.Repository.IRepository;
using AdminCompanyEmpManagementSystem.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AdminCompanyEmpManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public ManagementController(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper, UserManager<ApplicationUser> userManager,IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
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
            var passwordGen = "";
            ApplicationUser user = new ApplicationUser()
            {
                UserName = companyDTO.Email,
                PasswordHash = _userService.GeneratePassword(),
                Role = SD.Role_Company
            };
            passwordGen = user.PasswordHash;
            var registerCompany = await _userService.RegisterUser(user);
            companyDetail.ApplicationUserId = user.Id;
            if(_unitOfWork._companyRepository.FirstOrDefault(u=>u.GstNum == companyDetail.GstNum) != null)
            {
                return BadRequest(ModelState); 
            }
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
            // here we will send the email and in email we will send the ceredentials to the user
            _emailSender?.SendEmailAsync(user.UserName, "login Ceredentials",
                $"Your userId is {user.UserName} and password is {passwordGen}.");
            return Ok(new { Success = 1, Message = "Company Created Successfuly" });
        }


        //Summary:
        // Here we will update the company by both Admin and Company
        [Route("/UpdateCompany")]
        [HttpPut]
        public IActionResult UpdateCompany([FromBody] CompanyDTO companyDTO)
        {
            if (companyDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var companyDetail = _mapper.Map<Company>(companyDTO);
            
            // here check if we want to only update company and also its designation person.........
            if (!_unitOfWork._companyRepository.Update(companyDetail)) return BadRequest();
            // first we will get all the employee that alloted designation.......
            var EmpDesignationExist = _unitOfWork._allotedDesignationRepository.GetAll(u => u.CompanyId == companyDTO.Id);
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
                    // 
                    // alloted the desigantion to the company senior employee.
                    AllotedDesignationEmployee allotedDesigEmployee = new AllotedDesignationEmployee()
                    {
                        Name = companyDTO.CompanyDesigantion.ElementAt(i).Name?.Trim().ToString() ?? "",
                        CompanyId = companyDetail.Id, // static company id gave here we will work it later on.
                        DesignationId = designationId == 0 ? designationExist?.Id ?? 0 : designationId
                    };
                    var GetEmpDesigantionForUpdate = EmpDesignationExist.FirstOrDefault(u => u.DesignationId == designationExist?.Id);
                    if(GetEmpDesigantionForUpdate!=null)
                    {
                        allotedDesigEmployee = GetEmpDesigantionForUpdate;
                        allotedDesigEmployee.Name = companyDTO.CompanyDesigantion.ElementAt(i).Name?.Trim().ToString() ?? "";
                        EmpDesignationExist.Remove(GetEmpDesigantionForUpdate);
                    }
                    if (CreateNewDesignation)
                    {
                        if (!_unitOfWork._allotedDesignationRepository.Add(allotedDesigEmployee)) return BadRequest();
                        
                    }
                    else
                    {
                        if (!_unitOfWork._allotedDesignationRepository.Update(allotedDesigEmployee)) return BadRequest();

                    }
                }
            }
            
                if (EmpDesignationExist != null)
                {
                    _unitOfWork._allotedDesignationRepository.RemoveRange(EmpDesignationExist);
                }
            
            return Ok(new { Status = 1, Message = "Updated Successfully" });
        }



        //Summary:
        // here we will delete the company
        [Route("/DeleteCompany/{CmpId}")]
        [HttpDelete]
        public async  Task<IActionResult> DeleteCompany(int CmpId)
        {
            if (CmpId == 0) return BadRequest();
           
            // find the company
            var companyExist = _unitOfWork._companyRepository.FirstOrDefault(u=>u.Id== CmpId);
            if (companyExist == null) return NotFound();

            // then delete the employee of that company
            var deleteComEmployee = _unitOfWork._employeeRepository.GetAll(u => u.CompanyId == companyExist.Id);
            if (deleteComEmployee != null)
            {
                _unitOfWork._employeeRepository.RemoveRange(deleteComEmployee);
            }
            // delete company employee from the application user table
           for(int i=0;i<deleteComEmployee?.Count;i++)
            {
                var findInApplicationUser = await _userManager.FindByIdAsync(deleteComEmployee.ElementAt(i).ApplicationUserId);
                if (findInApplicationUser == null) return BadRequest();
                // remove applicationuser sae employee 
                if (!_userManager.DeleteAsync(findInApplicationUser).Result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            // find the employee designation of that company and delete it from the database
            var getEmpAllotedDesignation = _unitOfWork._allotedDesignationRepository.GetAll(u=>u.CompanyId == companyExist.Id);
            if (getEmpAllotedDesignation != null)
            {
                _unitOfWork._allotedDesignationRepository.RemoveRange(getEmpAllotedDesignation);
            }

            // here finally we delete company.
            if (!_unitOfWork._companyRepository.Delete(companyExist))
            {
            return StatusCode(StatusCodes.Status500InternalServerError);
            }
            // next we will delete company user it from application user.
            var findCompInApplicationUser = await _userManager.FindByIdAsync(companyExist.ApplicationUserId);
            if(findCompInApplicationUser == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            if (! _userManager.DeleteAsync(findCompInApplicationUser).Result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
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

        // Summary: get the company detail
        [Authorize]
        [Route("GetCompanyData")]
        [HttpGet]
        public async Task<IActionResult> getCompany()
        {
            // through claim we will get the company id and find the company in the database using this id and then we will return the company detail
            // through jwt token we will get the company id
            var claimIdentity = User.Identity as ClaimsIdentity;
            var claimUser = claimIdentity?.FindFirst(ClaimTypes.Name)?.Value??null;
            if (claimUser == null) return NotFound();
            var applicationUser = await _userManager.FindByIdAsync(claimUser);
            if(applicationUser == null) return NotFound();
            // then we will find the company 
            List<CompanyDTO> companyDetail = new List<CompanyDTO>();
           var getCompany = _unitOfWork._companyRepository?.FirstOrDefault(filter: u => u.ApplicationUserId == applicationUser.Id);
            if(getCompany==null) return NotFound();
                CompanyDTO company = new CompanyDTO()
                {
                    CompanyDesigantion = new List<CompanyDesignationDTO>()
                };
                var getAllDesEmp = _unitOfWork._allotedDesignationRepository.GetAll(u => u.CompanyId == getCompany.Id, includeTables: "Designation");
                company.Id = getCompany.Id;
                company.Name = getCompany.Name;
                company.Address = getCompany.Address;
                company.GstNum = getCompany.GstNum;
                company.ApplicationUserId = getCompany.ApplicationUserId;
                var companyDesigantion = new List<CompanyDesignationDTO>();
                foreach (var dataDesigantion in getAllDesEmp)
                {
                    var companyDesigantionDTO = new CompanyDesignationDTO()
                    {
                        Name = dataDesigantion.Name,
                        DesignationType = dataDesigantion.Designation.Name
                    };
                    companyDesigantion.Add(companyDesigantionDTO);
                }
                company.CompanyDesigantion.AddRange(companyDesigantion);

                 companyDetail.Add(company);
            
            return Ok(companyDetail);
        }


        #endregion

        





    }

}
