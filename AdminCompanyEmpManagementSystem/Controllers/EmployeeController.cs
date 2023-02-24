using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Models.DTOs;
using AdminCompanyEmpManagementSystem.Repository.IRepository;
using AdminCompanyEmpManagementSystem.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public EmployeeController(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper,UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Get(int empId)
        {
            if (empId == 0) return BadRequest();
            return Ok(new {Data = _unitOfWork._employeeRepository.FirstOrDefault(u=>u.Id == empId)});
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
            ApplicationUser user = new ApplicationUser()
            {
                UserName = employeeDTO.Name.Replace(" ","_"),
                PasswordHash = _userService.GeneratePassword(),
                Role = SD.Role_Employee
            };
            var registerCompany = await _userService.RegisterUser(user);
            if (employeeDTO == null || !ModelState.IsValid) return BadRequest();
            var EmployeeDetail = _mapper.Map<Employee>(employeeDTO);
            EmployeeDetail.ApplicationUserId = user.Id;
            if (!_unitOfWork._employeeRepository.Add(EmployeeDetail))return Ok(new { Success = 0, Message = "Employee not created" });
            return Ok(new { Success = 1, Message = "Created Successfully the Employee" });
        }
        [HttpPatch]
        public IActionResult Update([FromBody] EmployeeDTO employeeDTO)
        {
             
            if (employeeDTO == null || !ModelState.IsValid) return BadRequest();
            var EmployeeDetail = _mapper.Map<Employee>(employeeDTO);

            // check that the detail he update is correct or not.
            var checkAccNumber = _unitOfWork._employeeRepository.FirstOrDefault(u => u.AccountNum == employeeDTO.AccountNum);
            var checkPanNumber = _unitOfWork._employeeRepository.FirstOrDefault(u => u.PanNum == employeeDTO.PanNum);
            var checkPhoNumber = _unitOfWork._employeeRepository.FirstOrDefault(u=>u.PhoneNum == employeeDTO.PhoneNum);
            var checkpfNumber = _unitOfWork._employeeRepository.FirstOrDefault(u => u.PFNum == employeeDTO.PFNum);


           if((checkAccNumber!=null && checkAccNumber.Name!=employeeDTO.Name) || (checkPanNumber != null && checkPanNumber.Name != employeeDTO.Name) ||(checkpfNumber != null && checkpfNumber.Name != employeeDTO.Name) ||(checkPhoNumber != null && checkPhoNumber.Name != employeeDTO.Name))
            {
                return BadRequest(new { message = "Not update the record you enter the wrong ceredentials" });
            }
            // here we update the detail
            if (!_unitOfWork._employeeRepository.Update(EmployeeDetail)) return Ok(new { Success = 0, Message = "Employee not Updated" });
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
