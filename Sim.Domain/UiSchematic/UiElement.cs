using System.Xml.Linq;
using System.Collections.Generic;

namespace Sim.Domain.UiSchematic;

public record UiElement: ILogicElement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ElementClassName ClassName { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public bool Selected { get; set; } = false;
    public bool Mirror { get; set; } = false;
    public List<UiConnector> Connectors { get; set; } = [];

    public object ExtraProps { get; set; } // Can be IRelayExtraProps or ISwitcherExtraProps
    public object LogicState { get; set; } // Can be RelayLogicState or SwitcherLogicState

}

