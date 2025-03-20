using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using Sim.Api;
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
        service.AddTransient<MqttWorker>();
        service.AddTransient<MqttClientFactory>();
        service.AddTransient<MqttClientOptionsBuilder>();
    }

    static public void InitMqttServices(this WebApplication app)
    {
        var mqWorker = app.Services.GetRequiredService<MqttWorker>();

        app.Lifetime.ApplicationStarted.Register(() =>
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await mqWorker.PingAndSubscribeServer();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"MQTT connection error: {ex.Message}");
                }
            });
        });
    }
}
