using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Sim.Application.MqttServices;

sealed public class MqttServices
{
    private MqttServer? _mqttServer;
    private readonly ILogger<MqttServices> _logger;

    private string _lastClientId;

    public MqttServices(ILogger<MqttServices> logger)
    {
        _logger = logger;
    }

    public void InitMqttServer(MqttServer server)
    {
        _mqttServer = server;
    }

    public async Task OnClientConnected(ClientConnectedEventArgs args)
    {
        _logger.LogInformation($"Client connected: {args.ClientId}");
        
        if (_mqttServer == null) return;

        _lastClientId = args.ClientId;


        _ = Task.Run(async () =>
        {
            await Task.Delay(5000);
            await PublishToClient();
        });
    }

    public async Task PublishToClient()
    {
        if (_mqttServer == null) return;
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("ping")
            .WithPayload("Welcome from Server")
            .Build();

        await _mqttServer.InjectApplicationMessage(
            new InjectedMqttApplicationMessage(message)
            {
                SenderClientId = _lastClientId
            });
    }

    public Task ValidateConnection(ValidatingConnectionEventArgs eventArgs)
    {
        Console.WriteLine($"Client '{eventArgs.ClientId}' wants to connect. Accepting!");
        return Task.CompletedTask;
    }

    public Task OnIntercepted(InterceptingPublishEventArgs eventArgs)
    {
        Console.WriteLine($"Intercept topic '{eventArgs.ApplicationMessage.Topic}' and ClientId '{eventArgs.ClientId}' ");
        return Task.CompletedTask;
    }
}
