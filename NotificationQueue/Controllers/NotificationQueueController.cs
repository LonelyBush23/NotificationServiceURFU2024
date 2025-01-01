using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationQueue.Application.Features.Notification;

namespace NotificationQueue.Controllers
{
    [ApiController]
    [Route("/queue")]
    public class NotificationQueueController : ControllerBase
    {
        private readonly ILogger<NotificationQueueController> _logger;
        private readonly IMediator _mediator;


        public NotificationQueueController(ILogger<NotificationQueueController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("add")]
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
