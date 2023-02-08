namespace SignalR.API.Business.Abstract
{
    public interface INotificationService
    {
        public Task SendNotificationAsync(string message);
    }
}
