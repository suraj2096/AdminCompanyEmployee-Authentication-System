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
using System.Security.Claims;

namespace AdminCompanyEmpManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        
        
        public EmployeeController(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper,UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
            
            
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetEmployee()
        {
            // through claim we will get the company id and find the company in the database using this id and then we will return the company detail
            // through jwt token we will get the company id
            var claimIdentity = User.Identity as ClaimsIdentity;
            var claimUser = claimIdentity?.FindFirst(ClaimTypes.Name)?.Value ?? null;
            if (claimUser == null) return NotFound();
            var applicationUser = await _userManager.FindByIdAsync(claimUser);
            if (applicationUser == null) return NotFound();
            List<EmployeeDTO> getEmployeeList = new List<EmployeeDTO>();
             var getEmployee = _unitOfWork._employeeRepository.FirstOrDefault(u=>u.ApplicationUserId == applicationUser.Id,includeTables:"Company");
            if (getEmployee == null) return NotFound();
            getEmployeeList.Add(_mapper.Map<EmployeeDTO>(getEmployee));
            getEmployeeList.ElementAt(0).companyName = getEmployee.Company.Name;
            return Ok(new { Status = 1, Data = getEmployeeList });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeDTO employeeDTO)
        {
            var findEmpInOtherCompany = _unitOfWork._employeeRepository.FirstOrDefault(u=>u.AccountNum == employeeDTO.AccountNum || u.PFNum == employeeDTO.PFNum);
            if (findEmpInOtherCompany != null)
            {
                return BadRequest(new { Status = -1, Message = "Employee Already Exist in other Company!!!" });
            }
            // if company create employee then we will manullay register the employee by company.
            var passwordGen = "";
            ApplicationUser user = new ApplicationUser()
            {
                UserName = employeeDTO.Email,
                PasswordHash = _userService.GeneratePassword(),
                Role = SD.Role_Employee
            };
            passwordGen = user.PasswordHash;
            var registerCompany = await _userService.RegisterUser(user);
            if (employeeDTO == null || !ModelState.IsValid) return BadRequest();
            var EmployeeDetail = _mapper.Map<Employee>(employeeDTO);
            EmployeeDetail.ApplicationUserId = user.Id;
            if (!_unitOfWork._employeeRepository.Add(EmployeeDetail))return Ok(new { Success = 0, Message = "Employee not created" });
            // send the login ceredentials of employee through email
            /*_emailSender?.SendEmailAsync(user.UserName, "login Ceredentials",
               $"Your userId is {user.UserName} and password is {passwordGen}.");*/
            return Ok(new { Success = 1, Message = "Created Successfully the Employee", data = new { user = user.UserName, password = passwordGen } });
        }
        [NonAction]
        public bool CheckEmployeeExist(Employee employeeDetail,Employee getEmployee)
        {

            
            if (getEmployee?.AccountNum != employeeDetail.AccountNum) {

                if (_unitOfWork._employeeRepository.FirstOrDefault(u => u.AccountNum == employeeDetail.AccountNum) != null) return true;
            }
            else if(getEmployee?.PanNum != employeeDetail.PanNum && getEmployee?.PanNum != "")
            {
                if (_unitOfWork._employeeRepository.FirstOrDefault(u => u.PanNum == employeeDetail.PanNum) != null) return true;
            }

            else if (getEmployee?.PFNum != employeeDetail.PFNum)
            {
                if (_unitOfWork._employeeRepository.FirstOrDefault(u => u.PFNum == employeeDetail.PFNum) != null) return true;
            }
            else if (getEmployee?.PhoneNum != employeeDetail.PhoneNum){
                if (_unitOfWork._employeeRepository.FirstOrDefault(u => u.PhoneNum == employeeDetail.PhoneNum) != null) return true;
            }
            getEmployee.Name = employeeDetail.Name;
            getEmployee.Salary = employeeDetail.Salary;
            getEmployee.AccountNum = employeeDetail.AccountNum;
            getEmployee.PFNum = employeeDetail.PFNum;
            getEmployee.PhoneNum = employeeDetail.PhoneNum;
            getEmployee.PanNum= employeeDetail.PanNum;
            getEmployee.Address = employeeDetail.Address;
            getEmployee.Email = employeeDetail.Email;     
            return false;
            
        }
                
        [HttpPut]
        public IActionResult Update([FromBody] EmployeeDTO employeeDTO)
        {

             
            if (employeeDTO == null || !ModelState.IsValid) return BadRequest();
            var employeeDetail = _mapper.Map<Employee>(employeeDTO);

           var getEmployee = _unitOfWork._employeeRepository.FirstOrDefault(u => u.Id == employeeDTO.Id);
            if (getEmployee == null) return NotFound();

            var checkEmpExistInOtherCompany = CheckEmployeeExist(employeeDetail,getEmployee);
        if (checkEmpExistInOtherCompany) return BadRequest(new { message = "Not update the record you enter the wrong ceredentials" });

            if (!_unitOfWork._employeeRepository.Update(getEmployee)) return Ok(new { Success = 0, Message = "Employee not Updated" });
            return Ok(new { Success = 1, Message = "Updated Successfully the Employee" });
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return BadRequest();
            // now find the employee 
            var findEmployee = _unitOfWork._employeeRepository.FirstOrDefault(u => u.Id == id);
            if (findEmployee == null) return NotFound();
            // here delete the employee from employee table.
            if (!_unitOfWork._employeeRepository.Delete(findEmployee))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            // now we will delete it from appliationuser table  so its role will also delete 
            var findInApplicationUser = await _userManager.FindByIdAsync(findEmployee.ApplicationUserId);
            if (findInApplicationUser == null) return BadRequest();
            // remove applicationuser sae employee 
            if (!_userManager.DeleteAsync(findInApplicationUser).Result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new {Status=1, Message = "Deleted Successfully" });

        }

       
    }
}
