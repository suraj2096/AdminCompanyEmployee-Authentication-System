using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminCompanyEmpManagementSystem.Migrations
{
    public partial class someNameChangeinDesignationEmployeeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "DesignationEmployee",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "DesignationEmployee",
                newName: "name");
        }
    }
}
