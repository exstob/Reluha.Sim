using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain;

public class Connector
{
    public string Id { get; set; }
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public bool Connected { get; set; } = false;
    public List<string> JointConnectorsId { get; set; } = [];
    public List<string> JointBindersId { get; set; } = [];

}

