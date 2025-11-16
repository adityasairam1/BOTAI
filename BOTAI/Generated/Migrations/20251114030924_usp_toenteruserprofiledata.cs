using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOTAI.Generated.Migrations
{
    /// <inheritdoc />
    public partial class usp_toenteruserprofiledata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string script = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Scripts", "usp_toenteruserprofiledata.sql");
            var path = File.ReadAllText(script);
            migrationBuilder.Sql(path);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
