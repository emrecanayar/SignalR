using Microsoft.AspNetCore.SignalR;
using SignalR.API.Business.Abstract;
using SignalR.API.Hubs;

namespace SignalR.API.Business.Concrete
{
    public class NotificationManager : INotificationService
    {
        private readonly IHubContext<MyHub> _hubContext;

        public NotificationManager(IHubContext<MyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("Notification", $"Bu bir bildirimdir => {message}");
        }
    }
}
