using Sim.Domain.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.Dtos.Simulate;

public class RelayResult
{
    public required string Name { get; set; }
    public ContactValue NormalContact { get; init; }
    public ContactValue PolarContact { get; init; }
}
