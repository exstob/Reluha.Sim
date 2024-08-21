using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.UiSchematic;

public record UiSwitcher : UiElement
{
    public new UiSwitcherExtraProps ExtraProps { get; set; }
    public required new bool LogicState { get; set; }
}
