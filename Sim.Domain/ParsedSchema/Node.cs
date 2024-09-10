using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.ParsedScheme;

public class Node(string id) : ILogicEdge
{
    public string Id { get; init; } = id;
    public List<UiConnector> Connectors { get; init; } = [];
    public List<string> ElementNames { get; init; } = [];
    public IRelayEdge? RelayPin { get; set; }

    public bool Used { get; set; } = false;

    public override string ToString()
    {
        return string.Join("-", ElementNames);
    }

}
