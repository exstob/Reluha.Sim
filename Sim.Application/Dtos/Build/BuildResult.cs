using Sim.Application.Dtos.Simulate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.Dtos.Build;

public class BuildResult
{
    public required string SchemeId { get; set; }
    public required List<RelayLogicResult> Relays { get; set; }
}
