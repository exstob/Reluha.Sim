using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.CompressedScheme;

public class Node(int id) : IPin
{
    public int Id { get; init; } = id;
    //public List<Pin> Pins { get; init; }
    public List<UiConnector> Connectors { get; init; } = [];
    //public List<UiBinder> Binders { get; init; }

}
