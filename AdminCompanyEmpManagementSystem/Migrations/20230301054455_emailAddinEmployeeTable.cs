using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminCompanyEmpManagementSystem.Migrations
{
    public partial class emailAddinEmployeeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employee");
        }
    }
}
