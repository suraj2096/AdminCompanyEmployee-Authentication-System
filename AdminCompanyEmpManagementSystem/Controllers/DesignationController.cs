using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Models.DTOs;
using AdminCompanyEmpManagementSystem.Repository.IRepository;
using AdminCompanyEmpManagementSystem.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace AdminCompanyEmpManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public DesignationController(IUnitOfWork unitOfWork,IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Company)]
        [HttpGet("{cmpId:int}")]


        public async Task<IActionResult> GetDesignation(int cmpId)
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



            if (cmpId == 0) return BadRequest();
            var findCompany = _unitOfWork._companyRepository.FirstOrDefault(u=>u.Id == cmpId);
            if(findCompany == null) return NotFound();
            // now find alloted desigantion
            var allotedDesignation = _unitOfWork._allotedDesignationRepository.GetAll(u=>u.CompanyId== cmpId,includeTables:"Designation");
            if(allotedDesignation == null) return NotFound();   
            return Ok(new {Data=allotedDesignation});
        }


        [Authorize(Roles = SD.Role_Admin +","+SD.Role_Company)]
        [HttpPost]
        public async  Task<IActionResult> CreateUpdateDesignation([FromBody] List<CompanyDesignationDTO> desigantionList)
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





            foreach (var item in desigantionList)
            {
                if (item.EmpId == 0) return BadRequest();
                var findEmp = _unitOfWork._employeeRepository.FirstOrDefault(u=>u.Id == item.EmpId && u.CompanyId == item.CmpId);
                if (findEmp == null) return BadRequest(new {Message="You send wrong data"});
                // now first find Designation if designation is not then create desigantion
                var findDesigantion = _unitOfWork._designationRepository.FirstOrDefault(u => u.Name == item.DesignationType);
                    var designationId = 0;
                if(findDesigantion== null)
                {
                    // means we create a desiganation
                        Designation designation = new Designation()
                        {
                            Name = item.DesignationType?.Trim().ToString() ?? "",
                        };
                        var createDesignation = _unitOfWork._designationRepository.Add(designation);
                        if (createDesignation)
                        {
                            designationId = _unitOfWork._designationRepository.FirstOrDefault(u => u.Name == designation.Name)?.Id ?? 0;
                        }
                }
                // now we will check desigantion alloted to the employee or not
                var findAllotedEmpDesignation = _unitOfWork._allotedDesignationRepository.FirstOrDefault
                    (u => u.Name == findEmp.Name && u.CompanyId == item.CmpId);
                if(findAllotedEmpDesignation== null)
                {
                    // create code
                    AllotedDesignationEmployee allotedDesigEmployee = new AllotedDesignationEmployee()
                    {
                        Name = item.Name?.Trim().ToString() ?? "",
                        CompanyId = Convert.ToInt16(item.CmpId), // static company id gave here we will work it later on.
                        DesignationId = designationId == 0 ? findDesigantion?.Id ?? 0 : designationId
                    };
                    _unitOfWork._allotedDesignationRepository.Add(allotedDesigEmployee);
                }
                else
                {
                    findAllotedEmpDesignation.DesignationId = designationId == 0 ? findDesigantion?.Id ?? 0 : designationId;
                    _unitOfWork._allotedDesignationRepository.Update(findAllotedEmpDesignation);
                }

            }
            return Ok(new { Message = "Designation Updated Successfully" });
        }



        [Authorize(Roles = SD.Role_Admin +","+SD.Role_Company)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> RemoveDesigantion(int id)
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



            if (id == 0) return BadRequest();
            var findEmpInAlloDes = _unitOfWork._allotedDesignationRepository.FirstOrDefault(u => u.Id == id);
            if (findEmpInAlloDes == null) return NotFound();
            if (!_unitOfWork._allotedDesignationRepository.Delete(findEmpInAlloDes))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { Message = "Deleted Successfully" });
        }
    }
}
