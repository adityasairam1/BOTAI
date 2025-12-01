using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace BOTAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ChatController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest(new { message = "Message is required." });
            }

            var connectionString = _config.GetConnectionString("BOTAIDB");

            using var connection = new SqlConnection(connectionString);

            // Get all responses
            var responses = await connection.QueryAsync<ChatResponse>(
                "SELECT ResponseID, Keywords, ReplyText FROM BOTAI.ChatResponses"
            );

            // Find matching response based on keywords
            var userMessage = request.Message.ToLower();
            string reply = "I'm sorry, I don't have an answer for that. Could you ask about my work, skills, education, or experience?";

            int bestMatchCount = 0;
            string bestMatchReply = reply;

            // Check ALL responses and find the one with most keyword matches
            foreach (var response in responses)
            {
                var keywords = response.Keywords.ToLower().Split(',');
                int matchCount = 0;

                foreach (var keyword in keywords)
                {
                    var trimmedKeyword = keyword.Trim();
                    // Check if user message contains this keyword
                    if (userMessage.Contains(trimmedKeyword))
                    {
                        matchCount++;
                    }
                }

                // If this response has more matches than previous best, use it
                if (matchCount > bestMatchCount)
                {
                    bestMatchCount = matchCount;
                    bestMatchReply = response.ReplyText;
                }
            }

            // If we found at least one matching keyword, return the best match
            if (bestMatchCount > 0)
            {
                reply = bestMatchReply;
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