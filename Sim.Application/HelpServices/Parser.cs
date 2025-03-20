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
using Sim.Domain.ParsedSchema;


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
        do
        {
            if (!LogicBoxReducer.TryPackParallelContactBoxes(boxes, out var parBoxes))
            {
                if (!LogicBoxReducer.TrySimplifyTriangleBoxes(boxes, out parBoxes))
                {
                    //return (boxes, contacts);
                    break;
                }
                else if (!LogicBoxReducer.TryPackParallelContactBoxes(parBoxes, out parBoxes))
                {
                    //return (boxes, contacts);
                    break;
                }
            }

            if (!LogicBoxReducer.TryPackSerialContactBoxes(parBoxes, out var serialBoxes))
            {
                if (!LogicBoxReducer.TrySimplifyTriangleBoxes(parBoxes, out serialBoxes))
                {
                    //return (parBoxes, contacts);
                    boxes = serialBoxes;
                    break;
                }
            }
            boxes = serialBoxes;

        } while (true);

        if (!LogicBoxReducer.TryDefineRelayFanout(boxes, out var fanoutBoxes))
        {
            return (boxes, contacts);
        }
        return (fanoutBoxes, contacts);
    }

    public static List<Relay> ParseRelays(UiSchemeModel model, List<LogicBox> boxes)
    {
        return RelayCreator.Create(model, boxes);
    }

}
