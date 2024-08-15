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
    /// <summary>
    /// Create the Nodes
    /// </summary>
    /// <param name="model"></param>
    public static List<Node> ParseBinders(UiSchemeModel model)
    {
        return NodeCreator.CreateNodeFromJointBinders(model);
    }

    public static List<ContactBox> ParseSwitchers(UiSchemeModel model, List<Node> nodes)
    {
        var boxes = ContactBoxCreator.Create(model, nodes);
        if (ContactBoxReducer.TryPackParallelContactBoxes(boxes, out var parBoxes))
        {
            if (ContactBoxReducer.TryPackSerialContactBoxes(parBoxes, out var serialBoxes))
            {
                return serialBoxes;
            }
            return parBoxes;
        }

        return boxes;
    }

    public static List<Relay> ParseRelays(UiSchemeModel model, List<ContactBox> boxes)
    {
        return RelayCreator.Create(model, boxes);
    }









}
