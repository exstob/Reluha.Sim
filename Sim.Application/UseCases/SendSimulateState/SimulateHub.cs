using Microsoft.AspNetCore.SignalR;
using Sim.Application.MqttServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.UseCases.SendSimulateState;

public class SimulateHub : Hub
{
    private static readonly Dictionary<string, string> UserConnections = [];

    public override Task OnConnectedAsync()
    {
        //string userId = Context.UserIdentifier ?? "incognito"; // Assume you're using authentication

        var httpContext = Context.GetHttpContext();
        if (httpContext != null)
        {
            var schemeId = httpContext.Request.Query["schemeId"];
            if (!string.IsNullOrEmpty(schemeId))
            {
                UserConnections[schemeId] = Context.ConnectionId;
                Console.WriteLine($"Web socket is connected with schemeId: {schemeId}");
            }
            else
            {
                Console.WriteLine("schemeId not found in query parameters.");
            }
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        string userId = Context.UserIdentifier ?? "incognito";
        var httpContext = Context.GetHttpContext();
        if (httpContext != null)
        {
            var schemeId = httpContext.Request.Query["schemeId"];
            if (!string.IsNullOrEmpty(schemeId))
            {
                UserConnections.Remove(userId);
                Console.WriteLine($"Web socket is connected with schemeId: {schemeId}");
            }
            else
            {
                Console.WriteLine("schemeId not found in query parameters.");
            }
        }

        return base.OnDisconnectedAsync(exception);
    }

    public async Task PushSimulateState(string schemeId, SensorData data)
    {
        if (UserConnections.TryGetValue(schemeId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveSimulateState", data);
        }
        await Clients.All.SendAsync("ReceiveSimulateState", schemeId, data);
    }

    public async Task MessageFromWeb(string user, string message)
    {
        await Clients.All.SendAsync("messageReceived", user, message);
        await Task.Delay(2000);
        await Clients.All.SendAsync("messageReceived", user, "SECOND MESSAGE");

    }

    
}
