using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.Dtos.Build;

public class RelayLogicResult
{
    public required string Name { get; set; }
    public required string Logic { get; init; }
}
