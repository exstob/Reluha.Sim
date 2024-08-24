using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.Dtos.Simulate;

public class SimulateResult
{
    public required string SchemeId { get; set; }
    public required List<SimulateStepResult> Steps { get; set; }
}
