﻿using AdminCompanyEmpManagementSystem.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminCompanyEmpManagementSystem.Models.DTOs
{
    
    public class EmployeeDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public int Salary { get; set; }
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        public string PanNum { get; set; }
        [Required]
        public string AccountNum { get; set; }
        [Required]
        public string PFNum { get; set; }
        [Required]
        public string PhoneNum { get; set; }
        public int CompanyId { get; set; }
        public string? ApplicationUserId { get; set; }
        public string? companyName { get; set; }


    }
}
