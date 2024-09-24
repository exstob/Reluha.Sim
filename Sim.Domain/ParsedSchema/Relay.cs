using Sim.Domain.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.ParsedScheme;

public class Relay 
{
    public required string Name { get; set; }
    public required RelayState State { get; set; }
    public override string ToString() => Name;

}

