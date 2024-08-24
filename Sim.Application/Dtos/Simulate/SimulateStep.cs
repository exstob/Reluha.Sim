using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.Dtos.Simulate;

public class SimulateStep
{
    public required string StepName { get; set; }
    public List<UiSwitcher> Switchers { get; set; } = [];
    public List<ILogicElement> Inputs { get; set; } = [];
}