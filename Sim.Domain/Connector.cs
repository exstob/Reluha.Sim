using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain;

public class Connector
{
    public string Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public bool Connected { get; set; }
    public List<string> JointConnectorsId { get; set; } = [];
    public List<string> JointBindersId { get; set; } = [];

}

