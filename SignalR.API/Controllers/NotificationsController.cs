using Microsoft.AspNetCore.Mvc;
using SignalR.API.Business.Abstract;

namespace SignalR.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        //Abone olan client ların hepsine mesaj göndermemize yarar.
        [HttpPost(Name = "SendNotification")]
        public async Task<IActionResult> SendNotification(string message)
        {
            await _notificationService.SendNotificationAsync(message);
            return Ok();
        }
    }
}
