using Microsoft.AspNetCore.SignalR;

namespace BE.Hubs
{
    public class NotificationHub : Hub
    {
        public static int numberOfConnection = 0;

        public override async Task OnConnectedAsync()
        {
            numberOfConnection++;
            await base.OnConnectedAsync();
            System.Console.WriteLine($"Number Of Connection: {numberOfConnection}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            numberOfConnection--;
            await base.OnDisconnectedAsync(exception);
            System.Console.WriteLine($"Number Of Connection: {numberOfConnection}");
        }
    }
}
