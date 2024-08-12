using Sim.Domain.CompressedScheme;
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

    public static List<UiElement> AllPoles(this UiSchemeModel elements)
    {
        List<UiElement> allElements = [];
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

    public static (UiElement, UiConnector) NextElementAndConnector(this UiConnector connector, UiSchemeModel model)
    {
        var binder = model.Binders.Single(b => b.Id == connector.JointBindersId.Single());
        var nextConnectorId = binder.StartConnectorId == connector.Id
            ? binder.EndConnectorId
            : binder.StartConnectorId;

        var nextElement = model.AllElements().Single(el => el.Connectors.Any(c => c.Id == nextConnectorId));
        var nextConnector = nextElement.Connectors.Single(c => c.Id == nextConnectorId);

        return (nextElement, nextConnector);

    }


    public static bool IsMiniNode(this UiConnector connector) => connector.JointBindersId.Count > 1;

    public static bool IsCommon(this UiConnector connector) => connector.Name == ConnectorName.Common;
    public static bool IsOpen(this UiConnector connector) => connector.Name == ConnectorName.Open;
    public static bool IsClose(this UiConnector connector) => connector.Name == ConnectorName.Close;

    public static UiConnector CommonConnector(this UiSwitcher switcher) => switcher.Connectors.Single(c => c.Name == ConnectorName.Common);
    public static UiConnector OpenConnector(this UiSwitcher switcher) => switcher.Connectors.Single(c => c.Name == ConnectorName.Open);
    public static UiConnector CloseConnector(this UiSwitcher switcher) => switcher.Connectors.Single(c => c.Name == ConnectorName.Close);
    
    public static bool HasOpenContact(this UiSwitcher switcher) => switcher.CommonConnector()!.Connected && switcher.OpenConnector()!.Connected;
    public static bool HasOnlyOpenContact(this UiSwitcher switcher) => switcher.CommonConnector()!.Connected && switcher.OpenConnector()!.Connected && !switcher.CloseConnector()!.Connected;
    public static bool HasCloseContact(this UiSwitcher switcher) => switcher.CommonConnector()!.Connected && switcher.CloseConnector()!.Connected;
    public static bool HasOnlyCloseContact(this UiSwitcher switcher) => switcher.CommonConnector()!.Connected && switcher.CloseConnector()!.Connected && !switcher.OpenConnector()!.Connected;
    public static bool HasOnlyOneContact(this UiSwitcher switcher) => switcher.CommonConnector()!.Connected && (switcher.OpenConnector()!.Connected ^ switcher.CloseConnector()!.Connected);
    public static bool HasBothContacts(this UiSwitcher switcher) => HasOpenContact(switcher) && HasCloseContact(switcher);
}
