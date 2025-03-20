using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sim.Api.Dtos;

public class SensorData
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("schemeId")]
    public required string SchemeId { get; set; }
    [JsonPropertyName("normalContact")]
    public bool? NormalContact { get; init; }
    [JsonPropertyName("polarContact")]
    public bool? PolarContact { get; init; }
}
