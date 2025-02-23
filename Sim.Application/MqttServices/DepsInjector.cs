using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.MqttServices;

public static class DepsInjector
{
    static public void InjectMqttServices(this IServiceCollection service) 
    {
        service.AddSingleton<MqBroker>();
        service.AddSingleton<MqClient>();
        service.AddSingleton<MqttClientFactory>();
        service.AddSingleton<MqttClientOptionsBuilder>();
    }

    static public void InitMqttServices(this WebApplication app)
    {
        var mqClient = app.Services.GetRequiredService<MqClient>();

        app.Lifetime.ApplicationStarted.Register(() =>
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await mqClient.PingAndSubscribeServer();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"MQTT connection error: {ex.Message}");
                }
            });
        });
    }
}
