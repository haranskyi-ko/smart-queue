// Services/QueueHubService.cs
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;

namespace QueueApp.Client.Services;

public sealed class QueueHubService
{
    private readonly NavigationManager _nav;

    public HubConnection Connection { get; }

    public QueueHubService(NavigationManager nav)
    {
        _nav = nav;

        Connection = new HubConnectionBuilder()
            .WithUrl(_nav.ToAbsoluteUri("/queuehub"))
            .WithAutomaticReconnect()
            .Build();
    }

    public Task StartAsync() => Connection.StartAsync();
    public Task StopAsync() => Connection.StopAsync();
}
