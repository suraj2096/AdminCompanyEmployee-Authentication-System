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
        public ManagementController(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
        }


        #region Company Related work Here
        [Authorize(Roles = SD.Role_Admin)]
        [Route("/Admin/Companies")]
        [HttpGet]
        public async Task<IActionResult> GetAllCompnay()
        {
            // first we will get the claim and check it is valid user for this work
            ClaimsIdentity? claimIdentity = User?.Identity as ClaimsIdentity;
            if (claimIdentity == null) { return BadRequest(); }
            var claim = claimIdentity.FindFirst(ClaimTypes.Name);
            if (claim == null) { return BadRequest(); }
            var getUserDetailed = await _userService.CheckUserInDb(claim.Value);
            if (getUserDetailed == null) { return BadRequest(); }
            if (getUserDetailed.Role != SD.Role_Admin)
                return BadRequest();





            /* if(_userService.CheckUserInDb())*/
            List<CompanyDTO> companyDesigantionDto = new List<CompanyDTO>();
            var getAllCompany = _unitOfWork._companyRepository.GetAll();

            for (int i = 0; i < getAllCompany.Count; i++)
            {
                CompanyDTO company = new CompanyDTO()
                {
                    CompanyDesigantion = new List<CompanyDesignationDTO>()
                };
                var getAllDesEmp = _unitOfWork._allotedDesignationRepository.GetAll(u => u.CompanyId == getAllCompany.ElementAt(i).Id, includeTables: "Designation");
                var user = await _userManager.FindByIdAsync(getAllCompany.ElementAt(i).ApplicationUserId);
                company.Id = getAllCompany.ElementAt(i).Id;
                company.Name = getAllCompany.ElementAt(i).Name;
                company.Email = user.UserName;
                company.Email = getAllCompany.ElementAt(i).ApplicationUser.UserName;
                company.Address = getAllCompany.ElementAt(i).Address;
                company.GstNum = getAllCompany.ElementAt(i).GstNum;
                company.ApplicationUserId = getAllCompany.ElementAt(i).ApplicationUserId;
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
                companyDesigantionDto.Add(company);
            }
            return Ok(companyDesigantionDto);
        }



        // Summmary:
        /* In the Below Route /CreateCompany Here admin will create company and company will also create himself.
         and here we will create designation dynamically.
         */
        [Authorize(Roles = SD.Role_Admin)]
        [Route("/CreateCompany")]
        [HttpPost]

        public async Task<IActionResult> CreateCompany([FromBody] CompanyDTO companyDTO)
        {
            // first we will get the claim and check it is valid user for this work
            ClaimsIdentity? claimIdentity = User?.Identity as ClaimsIdentity;
            if (claimIdentity == null) { return BadRequest(); }
            var claim = claimIdentity.FindFirst(ClaimTypes.Name);
            if (claim == null) { return BadRequest(); }
            var getUserDetailed = await _userService.CheckUserInDb(claim.Value);
            if (getUserDetailed == null) { return BadRequest(); }
            if (getUserDetailed.Role != SD.Role_Admin)
                return BadRequest();





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
            if (_unitOfWork._companyRepository.FirstOrDefault(u => u.GstNum == companyDetail.GstNum) != null)
            {
                return BadRequest(ModelState);
            }
            if (!_unitOfWork._companyRepository.Add(companyDetail))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            // here we will send the email and in email we will send the ceredentials to the user
            /*_emailSender?.SendEmailAsync(user.UserName, "login Ceredentials",
                $"Your userId is {user.UserName} and password is {passwordGen}.");*/
            return Ok(new { Success = 1, Message = "Company Created Successfuly", data = new { user = user.UserName, password = passwordGen } });
        }


        //Summary:
        // Here we will update the company by both Admin and Company 
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Company)]
        [Route("/UpdateCompany")]
        [HttpPut]
        public async Task<IActionResult> UpdateCompany([FromBody] CompanyDTO companyDTO)
        {
            // first we will get the claim and check it is valid user for this work
            ClaimsIdentity? claimIdentity = User?.Identity as ClaimsIdentity;
            if (claimIdentity == null) { return BadRequest(); }
            var claim = claimIdentity.FindFirst(ClaimTypes.Name);
            if (claim == null) { return BadRequest(); }
            var getUserDetailed = await _userService.CheckUserInDb(claim.Value);
            if (getUserDetailed == null) { return BadRequest(); }
            if (getUserDetailed.Role != SD.Role_Admin || getUserDetailed.Role!=SD.Role_Company)
                return BadRequest();




            if (companyDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var companyDetail = _mapper.Map<Company>(companyDTO);
            
            // here we will update the company ...............
            if (!_unitOfWork._companyRepository.Update(companyDetail)) return BadRequest(); 
            return Ok(new { Status = 1, Message = "Updated Successfully" });
        }



        //Summary:
        // here we will delete the company
        [Authorize(Roles =SD.Role_Admin)]
        [Route("/DeleteCompany/{CmpId}")]
        [HttpDelete]
        public async  Task<IActionResult> DeleteCompany(int CmpId)
        {
            // first we will get the claim and check it is valid user for this work
            ClaimsIdentity? claimIdentity = User?.Identity as ClaimsIdentity;
            if (claimIdentity == null) { return BadRequest(); }
            var claim = claimIdentity.FindFirst(ClaimTypes.Name);
            if (claim == null) { return BadRequest(); }
            var getUserDetailed = await _userService.CheckUserInDb(claim.Value);
            if (getUserDetailed == null) { return BadRequest(); }
            if (getUserDetailed.Role != SD.Role_Admin)
                return BadRequest();








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
        [Authorize(Roles = SD.Role_Admin + ","+SD.Role_Company)]
        [Route("Company/Employee/{CmpId}")]
        [HttpGet]
        public async Task<IActionResult> GetCompanyEmployee(int CmpId)
        {
            // first we will get the claim and check it is valid user for this work
            ClaimsIdentity? claimIdentity = User?.Identity as ClaimsIdentity;
            if (claimIdentity == null) { return BadRequest(); }
            var claim = claimIdentity.FindFirst(ClaimTypes.Name);
            if (claim == null) { return BadRequest(); }
            var getUserDetailed = await _userService.CheckUserInDb(claim.Value);
            if (getUserDetailed == null) { return BadRequest(); }
            if (getUserDetailed.Role != SD.Role_Admin || getUserDetailed.Role != SD.Role_Company)
                return BadRequest();


            if (CmpId == 0) return BadRequest();
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
            // first we will get the claim and check it is valid user for this work
            ClaimsIdentity? claimIdentity = User?.Identity as ClaimsIdentity;
            if (claimIdentity == null) { return BadRequest(); }
            var claim = claimIdentity.FindFirst(ClaimTypes.Name);
            if (claim == null) { return BadRequest(); }
            var getUserDetailed = await _userService.CheckUserInDb(claim.Value);
            if (getUserDetailed == null) { return BadRequest(); }
            if (getUserDetailed.Role != SD.Role_Admin || getUserDetailed.Role != SD.Role_Company)
                return BadRequest();



            
            // then we will find the company 
            List<CompanyDTO> companyDetail = new List<CompanyDTO>();
           var getCompany = _unitOfWork._companyRepository?.FirstOrDefault(filter: u => u.ApplicationUserId == getUserDetailed.Id);
            if(getCompany==null) return NotFound();
                CompanyDTO company = new CompanyDTO()
                {
                    CompanyDesigantion = new List<CompanyDesignationDTO>()
                };
                var getAllDesEmp = _unitOfWork._allotedDesignationRepository.GetAll(u => u.CompanyId == getCompany.Id, includeTables: "Designation");
                company.Id = getCompany.Id;
            company.Email = getUserDetailed.UserName;
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
