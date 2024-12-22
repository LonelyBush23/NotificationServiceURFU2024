using Microsoft.AspNetCore.Mvc;

namespace NotificationQueue.Controllers
{
    [ApiController]
    [Route("/queue")]
    public class NotificationQueueController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddToQueue([FromBody] object requestBody)
        {
            return StatusCode(200, requestBody);
        }
    }
}
