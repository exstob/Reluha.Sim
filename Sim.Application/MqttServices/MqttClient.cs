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
using Sim.Application.Dtos.Simulate;
using Sim.Domain.UiSchematic;
using Sim.Application.UseCases.SimulateLogicModel;
using MQTTnet.Server;

namespace Sim.Application.MqttServices;

public class MqClient(MqttClientFactory factory, MqttClientOptionsBuilder builder)
{
    private IMqttClient _mqttClient = factory.CreateMqttClient();
    private readonly MqttClientOptions _option = builder
            .WithClientId("MyMqttClient")
            .WithWebSocketServer(o => o.WithUri("ws://reluhabroker.runasp.net:80/mqtt"))
            .Build();

    //private SimulateHub _hub = hub;
    //private ISimulateLogicModel _simulator = simulator;


    public async Task PublishAsync(string topic, string payload )
    {
        if (!_mqttClient.IsConnected)
            await _mqttClient.ConnectAsync(_option, CancellationToken.None);

        var message = new MqttApplicationMessageBuilder()
           .WithTopic(topic)
           .WithPayload(payload)
           .Build();
        await _mqttClient.PublishAsync(message, CancellationToken.None);
    }

    //public async Task PingAndSubscribeServer()
    //{
    //    if (!_mqttClient.IsConnected)
    //        await _mqttClient.ConnectAsync(_option, CancellationToken.None);

    //    //await _mqttClient.PingAsync(CancellationToken.None);
    //    await PublishAsync("reluhaready", "Hello, I am Reluha simulator");

    //    Console.WriteLine("The MQTT server replied to the ping request.");

    //    await SubscribeOnSensor();
    //}

    //public async Task SubscribeOnSensor()
    //{
    //    _mqttClient.ApplicationMessageReceivedAsync += async e =>
    //    {
    //        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
    //        Console.WriteLine($"Received application message: {payload}");


    //        var data = JsonSerializer.Deserialize<SensorData>(payload);
    //        if (data == null)
    //        {
    //            Console.WriteLine("Failed to deserialize the payload.");
    //            return;
    //        }

    //        var switchers = new List<UiSwitcher>();
    //        if (data.NormalContact.HasValue)
    //        {
    //            var normalSwitcher = new UiSwitcher()
    //            {
    //                Id = data.Id,
    //                Name = data.Name,
    //                ExtraProps = new UiSwitcherExtraProps("normal", false),
    //                LogicState = data.NormalContact.Value
    //            };
    //            switchers.Add(normalSwitcher);
    //            //_ = Task.Run(() => _hub.PushSwitchState(data.SchemeId, normalSwitcher));
    //            await _hub.PushSwitchState(data.SchemeId, normalSwitcher);
    //        }

    //        if (data.PolarContact.HasValue)
    //        {
    //            var polarSwitcher = new UiSwitcher()
    //            {
    //                Id = data.Id,
    //                Name = data.Name,
    //                ExtraProps = new UiSwitcherExtraProps("polar", false),
    //                LogicState = data.PolarContact.Value
    //            };
    //            switchers.Add(polarSwitcher);
    //            //_ = Task.Run(() => _hub.PushSwitchState(data.SchemeId, polarSwitcher));
    //            await _hub.PushSwitchState(data.SchemeId, polarSwitcher);
    //        }

    //        var simulateData = new SimulateData()
    //        {
    //            SchemeId = data.SchemeId,
    //            Steps = [new() {
    //                    StepName = $"Step-{data.Name}-{DateTime.Now}",
    //                    Switchers = switchers
    //                    }]
    //        };

    //        var result = await _simulator.Simulate(simulateData);


    //        //_ = Task.Run(() => _hub.PushSimulateState(result));
    //        await _hub.PushSimulateState(result);
    //    };


    //    var mqttSubscribeOptions = factory
    //        .CreateSubscribeOptionsBuilder()
    //        .WithTopicFilter("reluha/simulator")
    //        .Build();

    //    await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    //}
}
