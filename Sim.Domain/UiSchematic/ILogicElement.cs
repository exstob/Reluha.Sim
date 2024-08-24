using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.UiSchematic;

//public record LogicElement(string Id, string Name, object? ExtraProps, object LogicState);

public interface ILogicElement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public object? ExtraProps { get; set; } // This will hold either IRelayExtraProps or ISwitherExtraProps
    public object LogicState { get; set; } // This can be RelayLogicState, SwitcherLogicState, or bool
}
