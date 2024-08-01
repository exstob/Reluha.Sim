using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sim.Domain.UiSchematic.Extensions;
using Sim.Domain.CompressedScheme;
using System.Xml.Linq;
using Sim.Domain.Logic;
using System.Reflection;


namespace Sim.Application.NanoServices;

public static class Compressor
{
    /// <summary>
    /// Create the Nodes
    /// </summary>
    /// <param name="model"></param>
    public static List<Node> CompressBinders(UiSchemeModel model)
    {
        var nodeConnectors = model.AllElements()
            .Where(el => el.Connectors.Any(c => c.IsNode()))
            .SelectMany(el => el.Connectors).ToList();

        var allConnectors = model.AllElements().SelectMany(el => el.Connectors).ToList();

        var nodes = new List<Node>();

        for (int i = 0; i < nodeConnectors.Count; i++)
        {
            var nodeConnector = nodeConnectors.Last();
            var node = new Node(i);
            node.Connectors.Add(nodeConnector);

            //// if the connector was removed it means, it was procesed
            nodeConnectors.Remove(nodeConnector);
            allConnectors.Remove(nodeConnector);


            ////// Find all related connectors    _ _ _ 
            //////                                 | |
            //////
            var binderStack = new Stack<string>(nodeConnector.JointBindersId);

            while (binderStack.Count > 0)
            {
                var binderId = binderStack.Pop();
                var relatedConnectorId = model.Binders.FindNextConnectorId(binderId, nodeConnector.Id);

                /// Get a new connector if exist
                var relatedConnector = allConnectors.Find(c => c.Id == relatedConnectorId);

                if (relatedConnector?.IsNode() ?? false)
                {
                    /// push new binders for further calculation, exclude the currect binder
                    relatedConnector.JointBindersId.FindAll(id => id != binderId).ForEach(id => binderStack.Push(id));
                    node.Connectors.Add(relatedConnector);
                    nodeConnectors.Remove(relatedConnector);
                }

                if (relatedConnector != null)
                    allConnectors.Remove(relatedConnector);
            }

            nodes.Add(node);
        }

        var additionalNodes = CommonContactToNode(model);
        nodes.AddRange(additionalNodes);

        return nodes;
    }


    /// <summary>
    /// We should create the node from Common connector if we have the swither with both (open and close) connected contacts ____   ___
    /// </summary>                                                                                                               \
    /// <param name="model"></param>                                                                                              \ 
    /// <returns></returns>
    private static List<Node> CommonContactToNode(UiSchemeModel model)
    {
        var openCloseConnectedContacts = model.Switchers.Where(sw => sw.Connectors.All(c => c.Connected));
        var simpleCommonConnectors = openCloseConnectedContacts.Where(c => !c.Connectors.Common()?.IsNode() ?? false)
            .Select(c => c.Connectors.Common());
        return simpleCommonConnectors.Select((commonConnector, i) => 
        {
            var node = new Node(i+100); ///// REVIEW THIS
            node.Connectors.Add(commonConnector);

            var relatedConnectorId = model.Binders.FindNextConnectorId(commonConnector.JointBindersId.Single(), commonConnector.Id);
            var allConnectors = model.AllElements().SelectMany(el => el.Connectors).ToList();
            /// Get a new connector if exist
            var relatedConnector = allConnectors.Find(c => c.Id == relatedConnectorId);
            if (relatedConnector != null)
                node.Connectors.Add(relatedConnector);

            return node;
        }).ToList();

    }

    public static void CompressSwitchers(UiSchemeModel model)
    {
        var openCloseConnectedContacts = model.Switchers.Where(sw => sw.Connectors.All(c => c.Connected));
        var simpleCommonConnectors = openCloseConnectedContacts.Where(c => !c.Connectors.Common()?.IsNode() ?? false)
            .Select(c => c.Connectors.Common());

    }
}
