using Microsoft.AspNetCore.SignalR;
using Sim.Application.Dtos.Simulate;
using Sim.Application.MqttServices;
using Sim.Domain.UiSchematic;
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

    public async Task PushSwitchState(string schemeId, UiSwitcher switcher)
    {
        if (Clients == null)
        {
            Console.WriteLine("Workspace is unavailable");
            return;
        }

        if (UserConnections.TryGetValue(schemeId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveSwitchState", switcher);
        }
    }
    public async Task PushSimulateState(SimulateResult result)
    {
        if (Clients == null)
        {
            Console.WriteLine("Workspace is unavailable");
            return;
        }
        
        if (UserConnections.TryGetValue(result.SchemeId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveSimulateState", result);
        }
        //await Clients.All.SendAsync("ReceiveSimulateState", result.SchemeId, result);
    }

    public async Task MessageFromWeb(string user, string message)
    {
        await Clients.All.SendAsync("messageReceived", user, message);
        await Task.Delay(2000);
        await Clients.All.SendAsync("messageReceived", user, "SECOND MESSAGE");

    }

    
}
