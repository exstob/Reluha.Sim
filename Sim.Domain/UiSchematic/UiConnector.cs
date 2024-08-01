using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.UiSchematic;

public class UiConnector
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public bool Connected { get; set; }
    //public List<string> JointConnectorsId { get; set; } = [];
    public required List<string> JointBindersId { get; set; } = [];

}

