using AdminCompanyEmpManagementSystem.Models;
using AdminCompanyEmpManagementSystem.Models.DTOs;
using AdminCompanyEmpManagementSystem.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace AdminCompanyEmpManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public DesignationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{cmpId:int}")]
        public IActionResult GetDesignation(int cmpId)
        {
            if (cmpId == 0) return BadRequest();
            var findCompany = _unitOfWork._companyRepository.FirstOrDefault(u=>u.Id == cmpId);
            if(findCompany == null) return NotFound();
            // now find alloted desigantion
            var allotedDesignation = _unitOfWork._allotedDesignationRepository.GetAll(u=>u.CompanyId== cmpId,includeTables:"Designation");
            if(allotedDesignation == null) return NotFound();   
            return Ok(new {Data=allotedDesignation});
        }



        [HttpPost]
        public IActionResult CreateUpdateDesignation([FromBody] List<CompanyDesignationDTO> desigantionList)
        {
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
        [HttpDelete("{id:int}")]
        public IActionResult RemoveDesigantion(int id)
        {
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
