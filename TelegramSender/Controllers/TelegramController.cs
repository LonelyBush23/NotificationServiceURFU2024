using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelegramSender.Application;

namespace TelegramSender.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly TelegramService _telegramService;

        public TelegramController(TelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] TelegramRequest request)
        {
            try
            {
                await _telegramService.SendMessageAsync(request.ChatId, request.Text);
                return Ok(new { Message = "Message sent successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
    public class TelegramRequest
    {
        public string ChatId { get; set; }
        public string Text { get; set; }
    }
}
