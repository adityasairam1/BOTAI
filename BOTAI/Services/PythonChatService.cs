using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BOTAI.Services
{
    public class PythonChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pythonServiceBaseUrl;
        
        public PythonChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Set timeout to prevent hanging
            _httpClient.Timeout = System.TimeSpan.FromSeconds(30);
            
            // Get Python service URL from configuration (supports environment variable override)
            _pythonServiceBaseUrl = configuration["PythonService:BaseUrl"] 
                ?? Environment.GetEnvironmentVariable("PYTHON_SERVICE_URL") 
                ?? "http://localhost:9000";
            
            Console.WriteLine($"[DEBUG] PythonChatService: Using Python service URL: {_pythonServiceBaseUrl}");
        }
        
        public async Task<string> ProcessMessageAsync(int userId, string message)
        {
            try
            {
                var payload = new { user_id = userId, message = message };
                var jsonPayload = JsonSerializer.Serialize(payload);
                Console.WriteLine($"[DEBUG] PythonChatService: Sending request to Python service with payload: {jsonPayload}");
                
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var url = $"{_pythonServiceBaseUrl.TrimEnd('/')}/processMessage";
                Console.WriteLine($"[DEBUG] PythonChatService: Request URL: {url}");

                var response = await _httpClient.PostAsync(url, content);
                
                Console.WriteLine($"[DEBUG] PythonChatService: Response status: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] PythonChatService: Non-success status {response.StatusCode}. Error: {errorContent}");
                    throw new HttpRequestException($"Python service returned status {response.StatusCode}: {errorContent}");
                }

                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] PythonChatService: Response content: {result}");
                
                var json = JsonSerializer.Deserialize<JsonElement>(result);
                if (json.TryGetProperty("reply", out var replyProperty))
                {
                    return replyProperty.GetString() ?? string.Empty;
                }
                else
                {
                    Console.WriteLine($"[ERROR] PythonChatService: Response missing 'reply' property. Full response: {result}");
                    throw new InvalidOperationException("Python service response missing 'reply' property");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[ERROR] PythonChatService: HttpRequestException - {ex.Message}");
                throw;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"[ERROR] PythonChatService: Request timeout - {ex.Message}");
                throw new HttpRequestException("Python service request timed out", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] PythonChatService: Unexpected exception - {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}