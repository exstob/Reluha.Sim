using Sim.Domain.CompressedScheme;
using Sim.Domain.UiSchematic;
using Sim.Domain.UiSchematic.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.NanoServices;

static public class NodeCreator
{
    public static List<Node> CreateNodeFromJointBinders(UiSchemeModel model)
    {
        var nodeConnectors = model.AllElements()
            .Where(el => el.Connectors.Any(c => c.IsMiniNode()))
            .SelectMany(el => el.Connectors).ToList();

        ////// The Common connector also should be presented as Node 
        var simpleCommonConnectors = model.Switchers.Where(sw => sw.HasBothContacts() && !sw.CommonConnector().IsMiniNode())
            .Select(sw => sw.CommonConnector()).ToList();
        if (simpleCommonConnectors.Count > 0)
            nodeConnectors.AddRange(simpleCommonConnectors);

        var allConnectors = model.AllElements().SelectMany(el => el.Connectors).ToList();

        var nodes = new List<Node>();

        for (int i = 0; i < nodeConnectors.Count; i++)
        {
            var nodeConnector = nodeConnectors.Last();
            var node = new Node(nodeConnector.Id);
            node.Connectors.Add(nodeConnector);

            //// if the connector was removed it means, it was processed
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

                if (relatedConnector?.IsMiniNode() ?? false)
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

        return nodes;
    }

    /// <summary>
    /// We should create the node from CommonConnector connector if we have the switcher with both (open and close) connected contacts   ____   ___
    /// </summary>                                                                                                                           \
    /// <param name="model"></param>                                                                                                          \ 
    /// <returns></returns>
    private static List<Node> CreateNodeFromCommonContact(UiSchemeModel model)
    {
        //var openCloseConnectedContacts = model.Switchers.Where(sw => sw.Connectors.All(c => c.Connected));
        //var simpleCommonConnectors = openCloseConnectedContacts.Where(c => !c.CommonConnector()?.IsMiniNode() ?? false)
        //    .Select(c => c.CommonConnector());

        //// The Common connector also should be presented as Node 
        var simpleCommonConnectors = model.Switchers.Where(sw => sw.HasBothContacts() && !sw.CommonConnector().IsMiniNode())
            .Select(sw => sw.CommonConnector()).ToList();

        return simpleCommonConnectors.Select((commonConnector, i) =>
        {
            var node = new Node(commonConnector.Id);
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
}
