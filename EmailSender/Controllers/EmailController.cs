using EmailSender.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailSender.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            await _emailService.SendEmailAsync(request.Subject, request.Body, request.RecipientEmail);
            return Ok("Email sent successfully");
        }
    }

    public class EmailRequest
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string RecipientEmail { get; set; }
    }
}
