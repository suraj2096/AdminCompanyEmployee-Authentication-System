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
    public class LeaveController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LeaveController(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("{empId:int}")]
        public IActionResult getLeave(int empId)
        {
            if (empId == 0) return BadRequest();
            var findEmpLeave = _unitOfWork._leaveRepository.GetAll(u => u.EmpId == empId);
            if (findEmpLeave == null) return NotFound();
            return Ok(new { Data = findEmpLeave });
        }

        [HttpPost]
        public IActionResult createLeave([FromBody] LeaveDTO leaveDTO)
        {
            if (leaveDTO == null) return BadRequest();
            // map 
            var leave = _mapper.Map<Leaves>(leaveDTO);
            // first we will find the last leave he take is completed or not.
            // check start date and end date are in correct form
            if (leave.StartDate.Date > leaveDTO.EndDate.Date || leave.StartDate.Date < DateTime.Now)
            {
                return BadRequest(new { Message = "You give wrong input" });
            }


            var findlastleave = _unitOfWork._leaveRepository.GetAll(u => u.EmpId == leave.EmpId).LastOrDefault();
            if (findlastleave == null)
            {
                // here it means it is his first leave creation
                if (!_unitOfWork._leaveRepository.Add(leave))
                {
                    return BadRequest();
                }
                return Ok(new { Message = "Leave Created Successfully" });
            }
            if (findlastleave.EndDate > DateTime.Now && findlastleave.Status != (Leaves.LeaveStatus)3)
            {
                // this means it will not create another leave 
                return Ok(new { Message = "You take already a leave that not complete please check your leave on leave status button" });
            }
            // if last leave pass then we will create another leave
            if (!_unitOfWork._leaveRepository.Add(leave))
            {
                return BadRequest();
            }
            return Ok(new { Message = "Leave Created Successfully" });
        }

        [HttpGet("{status:int}/{cmpId:int}")]
        public IActionResult getLeaveByItsStatus(int status, int cmpId)
        {

            if (status == 0 || cmpId == 0) return BadRequest();
            // now we will get the employee leave list for specific company that are pending
            var getPendingLeave = _unitOfWork._leaveRepository.GetAll(u => u.Status == (Leaves.LeaveStatus)status && u.Employee.CompanyId == cmpId, includeTables: "Employee");
            if (getPendingLeave == null)
            {
                return Ok(new { Message = "no record" });
            }
            return Ok(new { Data = getPendingLeave });
        }
        [HttpPut]

        public IActionResult setApprovReject([FromBody] LeaveDTO leave)
        {
            if(leave == null) return BadRequest();
            var findLeave = _unitOfWork._leaveRepository.FirstOrDefault(u=>u.EmpId== leave.EmpId);
            if (findLeave == null) return BadRequest();
            findLeave.Status =(Leaves.LeaveStatus) leave.Status;
            if(!_unitOfWork._leaveRepository.Update(findLeave)) return BadRequest();
            return Ok(new { Data = "Leave Status Updated" });
        }
    }
}
