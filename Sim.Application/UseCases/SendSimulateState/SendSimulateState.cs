using Sim.Application.Dtos.Simulate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.UseCases.SendSimulateState;

internal class SendSimulateState
{
    private readonly Timer _timer;
    private readonly SimulateHub _hub;

    public SendSimulateState(SimulateHub hub)
    {
        _hub = hub;

        // Initialize a timer to call the Send method every 1 second (1000ms)
        _timer = new Timer(async _ => await Send(), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    }

    public async Task Send()
    {
        // Example user ID and message
        string userId = "userId";
        string message = "push";

        await _hub.PushSimulateState(userId, message);
    }

    public void StopTimer()
    {
        // Stop the timer when needed
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
    }
}
