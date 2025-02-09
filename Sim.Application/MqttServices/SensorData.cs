using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sim.Application.MqttServices;

public class SensorData
{
    public  string? Name { get; set; }
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("schemeId")]
    public required string SchemeId { get; set; }
    public bool NormalContact { get; init; }
    public bool PolarContact { get; init; }
}
