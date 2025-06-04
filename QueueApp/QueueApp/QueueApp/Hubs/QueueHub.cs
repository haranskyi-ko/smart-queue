using Microsoft.AspNetCore.SignalR;

namespace QueueApp.Server.Hubs
{
    public class QueueHub : Hub
    {
        //  ✔ Має бути public
        //  ✔ Повертає Task або ValueTask
        //  ✔ Назва EXACTLY така сама, як на клієнті
        public async Task JoinQueueGroup(string queueCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, queueCode);
            // (опціонально) підтвердження
            await Clients.Caller.SendAsync("Joined", queueCode);
        }

        public async Task LeaveQueueGroup(string queueCode)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, queueCode);
        }

        // інші методи...
    }
}
