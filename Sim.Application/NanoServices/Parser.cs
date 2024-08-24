using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sim.Domain.UiSchematic.Extensions;
using Sim.Domain.ParsedScheme;
using System.Xml.Linq;
using Sim.Domain.Logic;
using System.Reflection;
using System.Diagnostics;


namespace Sim.Application.NanoServices;

public static class Parser
{
    public static (List<Relay>, List<Contact>) Parse(UiSchemeModel model)
    {
        var nodes = ParseBinders(model);
        var (boxes, contacts) = ParseSwitchers(model, nodes);
        var relays = ParseRelays(model, boxes);
        return (relays, contacts);

    }

    /// <summary>
    /// Create the Nodes
    /// </summary>
    /// <param name="model"></param>
    public static List<Node> ParseBinders(UiSchemeModel model)
    {
        return NodeCreator.CreateNodeFromJointBinders(model);
    }

    public static (List<LogicBox>, List<Contact>) ParseSwitchers(UiSchemeModel model, List<Node> nodes)
    {
        var boxes = LogicBoxCreator.Create(model, nodes);
        var contacts = boxes.Where(b => b.Contacts is not null).SelectMany(b => b!.Contacts).ToList();
        if (LogicBoxReducer.TryPackParallelContactBoxesWithSameNodes(boxes, out var parBoxes))
        {
            if (LogicBoxReducer.TryPackSerialContactBoxes(parBoxes, out var serialBoxes))
            {
                if (LogicBoxReducer.TryPackParallelContactBoxesWithPoleAndNode(serialBoxes, out var poleBoxes))
                {
                    return (poleBoxes, contacts);
                }
                return (serialBoxes, contacts);
            }
            return (parBoxes, contacts);
        }

        return (boxes, contacts);
    }

    public static List<Relay> ParseRelays(UiSchemeModel model, List<LogicBox> boxes)
    {
        return RelayCreator.Create(model, boxes);
    }

}
