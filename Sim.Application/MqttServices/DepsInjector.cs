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
}
