using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminCompanyEmpManagementSystem.Migrations
{
    public partial class changeleavetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Leave",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Leave");
        }
    }
}
