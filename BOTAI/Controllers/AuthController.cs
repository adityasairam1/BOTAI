using BOTAI.DTO;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BOTAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var connectionString = _config.GetConnectionString("BOTAIDB");

            using var connection = new SqlConnection(connectionString);

            var user = await connection.QuerySingleOrDefaultAsync<UserLoginResult>(
                "BOTAI.LoginUser",
                new { Email = dto.Email, Password = dto.Password },
                commandType: CommandType.StoredProcedure
            );

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(new
            {
                message = "Login successful.",
                user
            });
        }

        // Internal class to receive DB result
        public class UserLoginResult
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}
