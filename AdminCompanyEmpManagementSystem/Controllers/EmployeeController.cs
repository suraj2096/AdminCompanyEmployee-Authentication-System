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
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public EmployeeController(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Get(int empId)
        {
            if (empId == 0) return BadRequest();
            return Ok(new {Data = _unitOfWork._employeeRepository.FirstOrDefault(u=>u.Id == empId)});
        }
        [HttpPost]
        public IActionResult Create([FromBody] EmployeeDTO employeeDTO)
        {
            if (employeeDTO == null || !ModelState.IsValid) return BadRequest();
            var EmployeeDetail = _mapper.Map<Employee>(employeeDTO);
            if (!_unitOfWork._employeeRepository.Add(EmployeeDetail))return Ok(new { Success = 0, Message = "Employee not created" });
            return Ok(new { Success = 1, Message = "Created Successfully the Employee" });
        }
        [HttpPatch]
        public IActionResult Update([FromBody] EmployeeDTO employeeDTO)
        {
            if (employeeDTO == null || !ModelState.IsValid) return BadRequest();
            var EmployeeDetail = _mapper.Map<Employee>(employeeDTO);
            if (!_unitOfWork._employeeRepository.Update(EmployeeDetail)) return Ok(new { Success = 0, Message = "Employee not Updated" });
            return Ok(new { Success = 1, Message = "Updated Successfully the Employee" });
        }
       
    }
}
