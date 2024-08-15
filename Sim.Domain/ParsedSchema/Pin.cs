using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.ParsedScheme;

public class Pin(string id) : ILogicEdge
{
    public string Id { get; init; } = id;
}
