using Microsoft.EntityFrameworkCore.Migrations;

namespace ExamsSchedule.Migrations.DeepsensorClient
{
    public partial class AddnewTableDashboard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "DashboardItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "DashboardItems");
        }
    }
}
