using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using System.Text.Json;
using Sim.Application.UseCases.SendSimulateState;

namespace Sim.Application.MqttServices;

public class MqClient(MqttClientFactory factory, MqttClientOptionsBuilder builder, SimulateHub hub)
{
    private IMqttClient _mqttClient = factory.CreateMqttClient();
    private readonly MqttClientOptions _option = builder
            .WithClientId("MyMqttClient")
            .WithWebSocketServer(o => o.WithUri("ws://reluhabroker.runasp.net:80/mqtt"))
            .Build();

    private SimulateHub _hub = hub;

    //private readonly JsonSerializerOptions serializeOptions = new () { PropertyNameCaseInsensitive = true };

public async Task Ping_Server()
    {
        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine($"Received application message: {payload}");

            var data = JsonSerializer.Deserialize<SensorData>(payload);

            await _hub.PushSimulateState(data.SchemeId, data);

        };

        if (!_mqttClient.IsConnected)
            await _mqttClient.ConnectAsync(_option, CancellationToken.None);

        //await _mqttClient.PingAsync(CancellationToken.None);

         var message = new MqttApplicationMessageBuilder()
            .WithTopic("reluhaready")
            .WithPayload("Hello, I am Reluha simulator")
            .Build();

        await _mqttClient.PublishAsync(message, CancellationToken.None);

        Console.WriteLine("The MQTT server replied to the ping request.");


        var mqttSubscribeOptions = factory
            .CreateSubscribeOptionsBuilder()
            .WithTopicFilter("reluha/simulator")
            .Build();

        await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    }
}
