using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using QueueApp.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);



builder.Services.AddScoped(
    _ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
