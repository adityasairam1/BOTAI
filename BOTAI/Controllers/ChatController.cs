using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using BOTAI.Services;

namespace BOTAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly PythonChatService _pythonChatService;

        public ChatController(IConfiguration config, PythonChatService pythonChatService)
        {
            _config = config;
            _pythonChatService = pythonChatService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            Console.WriteLine($"[DEBUG] Received message: {request.Message}");

            if (string.IsNullOrEmpty(request.Message))
            {
                Console.WriteLine("[DEBUG] Message was empty");
                return BadRequest(new { message = "Message is required." });
            }

            string reply = null;

            try
            {
                var connectionString = _config.GetConnectionString("BOTAIDB");
                using var connection = new SqlConnection(connectionString);

                var responses = await connection.QueryAsync<ChatResponse>(
                    "SELECT ResponseID, Keywords, ReplyText FROM BOTAI.ChatResponses"
                );

                Console.WriteLine($"[DEBUG] Retrieved {responses.Count()} responses from DB");

                var userMessage = request.Message.ToLower();
                int bestMatchCount = 0;

                foreach (var response in responses)
                {
                    var keywords = response.Keywords.ToLower().Split(',');
                    int matchCount = keywords.Count(k => userMessage.Contains(k.Trim()));

                    if (matchCount > bestMatchCount)
                    {
                        bestMatchCount = matchCount;
                        reply = response.ReplyText;
                    }
                }

                if (bestMatchCount == 0)
                {
                    Console.WriteLine("[DEBUG] No DB match found. Calling Python service...");
                    reply = await _pythonChatService.ProcessMessageAsync(1, request.Message);
                    Console.WriteLine($"[DEBUG] Python service replied: {reply}");
                }
                else
                {
                    Console.WriteLine($"[DEBUG] DB match found. Reply: {reply}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception in ChatController: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }

            if (string.IsNullOrEmpty(reply))
            {
                reply = "I'm sorry, I don't have an answer for that. Could you ask about my work, skills, education, or experience?";
                Console.WriteLine("[DEBUG] Fallback reply used.");
            }

            return Ok(new { reply });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = default!;
    }

    public class ChatResponse
    {
        public int ResponseID { get; set; }
        public string Keywords { get; set; } = default!;
        public string ReplyText { get; set; } = default!;
    }
}