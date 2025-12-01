using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOTAI.Generated.Migrations
{
    /// <inheritdoc />
    public partial class _002_Create_Login_Procedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string script = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Scripts", "002_Create_Login_Procedure.sql");
            var path = File.ReadAllText(script);
            migrationBuilder.Sql(path);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
