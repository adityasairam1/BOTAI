using BOTAI.DTO;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BOTAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignupController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        public SignupController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDTO dto)
        {
            var connectionString = Configuration.GetConnectionString("BOTAIDB");

            using var connection = new SqlConnection(connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", dto.UserName);
            parameters.Add("@Password", dto.Password); // Consider hashing!
            parameters.Add("@Email", dto.Email);
            parameters.Add("@FirstName", dto.FirstName);
            parameters.Add("@LastName", dto.LastName);

            try
            {
                await connection.ExecuteAsync("BOTAI.CreateUserProfile", parameters, commandType: CommandType.StoredProcedure);
                return Ok(new { message = "User created successfully." });
            }
            catch (SqlException ex) when (ex.Number == 50000) // RAISERROR
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the user." });
            }
        }
    }
}
