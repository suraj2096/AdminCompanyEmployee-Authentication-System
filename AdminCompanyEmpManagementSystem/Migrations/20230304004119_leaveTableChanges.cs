using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminCompanyEmpManagementSystem.Migrations
{
    public partial class leaveTableChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leave_Employee_EmployeeId",
                table: "Leave");

            migrationBuilder.DropIndex(
                name: "IX_Leave_EmployeeId",
                table: "Leave");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Leave");

            migrationBuilder.DropColumn(
                name: "LeaveStatus",
                table: "Leave");

            migrationBuilder.RenameColumn(
                name: "TotalLeave",
                table: "Leave",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "LeaveDate",
                table: "Leave",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Leave",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Leave_EmpId",
                table: "Leave",
                column: "EmpId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leave_Employee_EmpId",
                table: "Leave",
                column: "EmpId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leave_Employee_EmpId",
                table: "Leave");

            migrationBuilder.DropIndex(
                name: "IX_Leave_EmpId",
                table: "Leave");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Leave");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Leave",
                newName: "TotalLeave");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Leave",
                newName: "LeaveDate");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Leave",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LeaveStatus",
                table: "Leave",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Leave_EmployeeId",
                table: "Leave",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leave_Employee_EmployeeId",
                table: "Leave",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
