using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.Dtos.Simulate;

public class SimulateStepResult
{
    public required string StepName { get; set; }
    public List<RelayResult> Relays { get; set; } = [];
    public List<ChainValue> Outputs { get; set; } = [];
}
