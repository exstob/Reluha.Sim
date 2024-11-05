using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.UseCases.GetCircuitUC;

public class CircuitResult
{
    public required string Name { get; set; }
    public required string Content { get; init; }
}