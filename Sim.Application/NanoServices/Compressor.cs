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
            .Where(el => el.Connectors.Any(c => c.IsMiniNode()))
            .SelectMany(el => el.Connectors).ToList();

        //var openCloseConnectedContacts = model.Switchers.Where(sw => sw.Connectors.All(c => c.Connected));
        //var simpleCommonConnectors = openCloseConnectedContacts.Where(c => !c.Connectors.Common()?.IsMiniNode() ?? false)
        //    .Select(c => c.Connectors.Common()).Where(connector => connector != null).ToList();
        //if (simpleCommonConnectors.Count > 0)
        //    nodeConnectors.AddRange(simpleCommonConnectors);

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

        //var additionalNodes = CommonContactToNode(model);
        //nodes.AddRange(additionalNodes);

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
        var simpleCommonConnectors = openCloseConnectedContacts.Where(c => !c.Common()?.IsMiniNode() ?? false)
            .Select(c => c.Common());
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

    public static void CompressSwitchers(UiSchemeModel model, List<Node> nodes)
    {
        //var connectedSwitchers = model.Switchers.Where(sw => sw.Connectors.Common()!.Connected && (sw.Connectors.Open()!.Connected || sw.Connectors.Close()!.Connected)).ToList();
        var connectedSwitchers = model.Switchers.Where(sw => sw.HasCloseContact() || sw.HasOpenContact()).ToList();

        /// Analyze routes from Common contact

        var fullSwitchers = model.Switchers.Where(sw => sw.HasBothContacts());
        foreach (var switcher in fullSwitchers)
        {

            /// Calc route via Open contact
            var box = new ContactBox(ContactBoxSort.Selial);
            var startConnector = switcher.Common();
            List<Contact> contacts = [new Contact(switcher, ContactDefaultState.Open)];
            var serialSwitchers = TryFindNextSerialSwitcher(switcher.Open(), connectedSwitchers, model.Binders, nodes).ToList();
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
            }

            box.FirstPin = startConnector;
            box.SecondPin = serialSwitchers.Last().connector;
            box.Contacts = contacts;


            /// Calc route via Close contact
            contacts = [new Contact(switcher, ContactDefaultState.Close)];
            serialSwitchers = TryFindNextSerialSwitcher(switcher.Close(), connectedSwitchers, model.Binders, nodes).ToList();
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
            }

            box.FirstPin = startConnector;
            box.SecondPin = serialSwitchers.Last().connector;
            box.Contacts = contacts;


            connectedSwitchers.Remove(switcher);
        }

        /// Analyze routes from Node

        /// Analyze routes from  Pole => Node path
        foreach (var pole in model.PosPoles.FindAll(p => p.Connectors.FirstOrDefault()?.Connected ?? false))
        {
            var box = new ContactBox(ContactBoxSort.Selial);
            var startConnector = pole.Connectors.Single();
            var serialSwitchers = TryFindNextSerialSwitcher(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
            List<Contact> contacts = [];
            foreach((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers) 
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
            }

            box.FirstPin = startConnector;
            box.SecondPin = serialSwitchers.Last().connector;
            box.Contacts = contacts;
        }

        /// Analyze routes from Relay path 
    }





    static IEnumerable<(UiSwitcher sw, bool viaOpen, UiConnector connector)> TryFindNextSerialSwitcher(UiConnector startConnector, List<UiSwitcher> switchers, List<UiBinder> binders, List<Node> nodes)
    {
        UiConnector connector = startConnector;
        //bool runLoop = true;

        while (true) 
        {

            var node = nodes.Find(n => n.Connectors.Contains(connector));
            if (node is null) yield break; //// detect the end of serial chain.

            var binderId = connector.JointBindersId.Single();
            var binder = binders.Single(b => b.Id == binderId);
            var nextConnectorId = binder.StartConnectorId == connector.Id
                ? binder.EndConnectorId
                : binder.StartConnectorId;

            var switcher = switchers.Find(el => el.Connectors.Any(c => c.Id == nextConnectorId));
            if (switcher is null) yield break; /// it mean we have connection with another non-switcher element
            UiConnector switcherConnector = switcher.Connectors.Single(c => c.Id == nextConnectorId);

            if (switcher.HasBothContacts() && switcherConnector.IsCommon()) yield break;  /// Common connector is like Node, thus, we should cut the chain

            UiConnector? nextConnector =
                switcher.HasOpenContact() && switcherConnector.IsOpen() ? switcher.Common()
                : switcher.HasOpenContact() && switcherConnector.IsCommon() ? switcher.Open()
                : switcher.HasCloseContact() && switcherConnector.IsClose() ? switcher.Common() 
                : switcher.HasCloseContact() && switcherConnector.IsCommon() ? switcher.Close() 
                : null;
 

            //runLoop = nextConnector is not null;
            if (nextConnector is not null)
            {
                connector = nextConnector!;
                
                yield return (switcher, switcher.HasOpenContact(), connector);
            }

            if(nextConnector is not null && switcher.HasBothContacts() && switcherConnector.IsCommon()) /// Common connector is like Node, thus, we should cut the chain
                yield break;

            
        }

    }


}
