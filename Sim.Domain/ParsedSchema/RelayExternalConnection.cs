using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.ParsedSchema;

public class RelayExternalConnection
{
    public string MqttTopic { get; set; }
    public string HttpPath { get; set; }
}
