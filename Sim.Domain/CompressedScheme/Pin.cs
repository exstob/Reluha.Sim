using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.CompressedScheme;

public class Pin : ILogicEdge
{ 
    public required object Parent { get; init; }
    public string Name { get; init; }
    public Pin?  CoupledPin { get; init; }
    public Node? CoupledNode { get; init; }
}
