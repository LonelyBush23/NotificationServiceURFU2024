using MediatR;
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Application.Features.Notification;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("/notification")]
    public class NotificationController : ControllerBase
    {
        ILogger<NotificationController> _logger;
        private readonly IMediator _mediator;


        public NotificationController(ILogger<NotificationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("send")]
        public async Task<IActionResult> AddToQueue([FromBody] SendNotificationCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccessfull)
            {
                return BadRequest(result.GetErrors().FirstOrDefault());
            }
            return Ok();
        }
    }
}
