using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatbotController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatbotController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
                return BadRequest(new { error = "Vui lòng nhập câu hỏi." });

            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
                return Ok(new { reply = "⚙️ Chatbot AI chưa được cấu hình API Key. Vui lòng liên hệ quản trị viên." });

            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var systemPrompt = @"Bạn là Trợ lý Y tế AI của HUTECH_Hospital - một bệnh viện số thông minh tại HUTECH Khu E, TP.Thủ Đức.
Nhiệm vụ của bạn là hỗ trợ bệnh nhân và người dùng với các câu hỏi về:
- Thông tin về các chuyên khoa và dịch vụ y tế tại bệnh viện
- Hướng dẫn đặt lịch khám bệnh
- Triệu chứng bệnh thông thường và lời khuyên sức khỏe ban đầu (KHÔNG thay thế bác sĩ)
- Thông tin về các bác sĩ và chuyên môn
- Quy trình, chi phí khám bệnh
- Chính sách bệnh viện

Trả lời bằng tiếng Việt, thân thiện, chuyên nghiệp và ngắn gọn. 
QUAN TRỌNG: Luôn khuyên bệnh nhân gặp bác sĩ trực tiếp để được chẩn đoán chính xác. 
Không cung cấp chẩn đoán y tế hoặc kê đơn thuốc.";

            var payload = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = systemPrompt } }
                },
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[] { new { text = request.Message } }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 512,
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return Ok(new { reply = $"❌ Có lỗi kết nối tới AI: {err}" });
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var reply = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return Ok(new { reply });
        }

        public class ChatRequest
        {
            public string? Message { get; set; }
        }
    }
}
