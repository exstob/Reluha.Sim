using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.UiSchematic.Extensions;

public static class UiSchemeExtension
{
    public static List<UiElement> AllElements(this UiSchemeModel elements)
    {
        List<UiElement> allElements = [];
        allElements.AddRange(elements.Relays);
        allElements.AddRange(elements.Switchers);
        allElements.AddRange(elements.NegPoles);
        allElements.AddRange(elements.PosPoles);
        return allElements;
    }

    public static string FindNextConnectorId(this List<UiBinder> binders, string binderId, string connectorId)
    {
        var binder = binders.Single(b => b.Id == binderId);
        return binder.StartConnectorId == connectorId
            ? binder.EndConnectorId
            : binder.StartConnectorId;

    }

    public static bool IsNode(this UiConnector connector) => connector.JointBindersId.Count > 1;

    public static UiConnector? Common(this List<UiConnector> connectors) => connectors.Find(c => c.Name == ConnectorName.Common);
}
