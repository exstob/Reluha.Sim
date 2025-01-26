using Microsoft.AspNetCore.SignalR;
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
        string userId = Context.UserIdentifier; // Assume you're using authentication
        UserConnections[userId] = Context.ConnectionId;
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        string userId = Context.UserIdentifier;
        UserConnections.Remove(userId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task PushSimulateState(string userId, string message)
    {
        if (UserConnections.TryGetValue(userId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }
    }

    public async Task MessageFromWeb(string user, string message)
    {
        await Clients.All.SendAsync("messageReceived", user, message);
        await Task.Delay(2000);
        await Clients.All.SendAsync("messageReceived", user, "SECOND MESSAGE");

    }

    
}
