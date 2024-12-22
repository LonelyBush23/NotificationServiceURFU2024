using System.Runtime.CompilerServices;
using ApiGateway.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public NotificationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();

            var _targetUrl = configuration["NotificationQueue:BaseUrl"] ?? throw new ArgumentNullException("BaseUrl", "BaseUrl is not configured in appsettings.json");
            _httpClient.BaseAddress = new Uri(_targetUrl);
        }

        [HttpPost("/send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationDTO requestBody)
        {
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync("/queue", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, responseContent);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
