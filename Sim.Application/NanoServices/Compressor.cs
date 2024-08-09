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
using System.Diagnostics;


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
        var connectedSwitchers = model.Switchers.Where(sw => sw.HasCloseContact() || sw.HasOpenContact()).ToList();
        List<ContactBox> boxes = [];
        
        /// Analyze routes from Common contact
        var (childBoxes, usedSwitchers) = CreateBoxesFromCommonContacts(connectedSwitchers, model, nodes);
        boxes.AddRange(childBoxes);
        connectedSwitchers.RemoveAll(csw => usedSwitchers.Contains(csw));

        /// Analyze routes from  Pole => Node path
        (boxes, usedSwitchers) = CreateBoxesFromPole(connectedSwitchers, model, nodes);
        boxes.AddRange(childBoxes);
        connectedSwitchers.RemoveAll(csw => usedSwitchers.Contains(csw));

        /// Analyze routes from Relay path 
        (boxes, usedSwitchers) = CreateBoxesFromRelay(connectedSwitchers, model, nodes);
        boxes.AddRange(childBoxes);
        connectedSwitchers.RemoveAll(csw => usedSwitchers.Contains(csw));

        /// Analyze routes from Node to Node
        (boxes, usedSwitchers) = CreateBoxesFromNode(connectedSwitchers, model, nodes);
        boxes.AddRange(childBoxes);
        connectedSwitchers.RemoveAll(csw => usedSwitchers.Contains(csw));


    }


    private static (List<ContactBox> boxes, List<UiSwitcher> switchers) CreateBoxesFromCommonContacts(List<UiSwitcher> connectedSwitchers, UiSchemeModel model, List<Node> nodes)
    {
        List<ContactBox> boxes = [];
        List<UiSwitcher> usedSwitchers = [];
        var fullSwitchers = connectedSwitchers.Where(sw => sw.HasBothContacts());
        foreach (var switcher in fullSwitchers)
        {

            /// Calc route via Open contact
            var startConnector = switcher.Common();
            List<Contact> contacts = [new Contact(switcher, ContactDefaultState.Open)];
            var serialSwitchers = TryFindNextSerialSwitchers(switcher.Open(), connectedSwitchers, model.Binders, nodes).ToList();
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
            }

            
            var box1 = new ContactBox(ContactBoxSort.Serial);
            box1.FirstPin = GetLogicEdge(startConnector, model, nodes); //// REVIEW THIS
            box1.SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes);
            box1.Contacts = contacts;
            boxes.Add(box1);


            /// Calc route via Close contact
            contacts = [new Contact(switcher, ContactDefaultState.Close)];
            serialSwitchers = TryFindNextSerialSwitchers(switcher.Close(), connectedSwitchers, model.Binders, nodes).ToList();
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
            }

            var box2 = new ContactBox(ContactBoxSort.Serial);
            box2.FirstPin = GetLogicEdge(startConnector, model, nodes); //// REVIEW THIS
            box2.SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes);
            box2.Contacts = contacts;
            boxes.Add(box2);

            usedSwitchers.Add(switcher);
        }

        return (boxes, usedSwitchers);
    }


    private static (List<ContactBox> boxes, List<UiSwitcher> switchers) CreateBoxesFromPole(List<UiSwitcher> connectedSwitchers, UiSchemeModel model, List<Node> nodes)
    {
        List<ContactBox> boxes = [];
        List<UiSwitcher> usedSwitchers = [];
        ////From positive pole
        foreach (var pole in model.PosPoles.FindAll(p => p.Connectors.FirstOrDefault()?.Connected ?? false))
        {
            var box = new ContactBox(ContactBoxSort.Serial);
            var startConnector = pole.Connectors.Single();
            var serialSwitchers = TryFindNextSerialSwitchers(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
            List<Contact> contacts = [];
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
                usedSwitchers.Add(sw);
            }
            if (contacts.Count == 0) continue;
            box.FirstPin = new PolePositive();
            box.SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes);
            box.Contacts = contacts;
            boxes.Add(box);
        }

        ////From negative pole
        foreach (var pole in model.NegPoles.FindAll(p => p.Connectors.FirstOrDefault()?.Connected ?? false))
        {
            var box = new ContactBox(ContactBoxSort.Serial);
            var startConnector = pole.Connectors.Single();
            var serialSwitchers = TryFindNextSerialSwitchers(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
            List<Contact> contacts = [];
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
                usedSwitchers.Add(sw);
            }
            if (contacts.Count == 0) continue;
            box.FirstPin = new PoleNegative();
            box.SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes);
            box.Contacts = contacts;
            boxes.Add(box);
        }

        return (boxes, usedSwitchers);
    }

    private static (List<ContactBox> boxes, List<UiSwitcher> switchers) CreateBoxesFromRelay(List<UiSwitcher> connectedSwitchers, UiSchemeModel model, List<Node> nodes)
    {
        List<ContactBox> boxes = [];
        List<UiSwitcher> usedSwitchers = [];
        var connectedRelays = model.Relays.FindAll(p => p.Connectors.All(c => c.Connected));
        ////From positive pole
        foreach (var relay in connectedRelays)
        {
            /// Calc route via First (Plus) contact  of Relay
            var startConnector = relay.Connectors.Single();
            List<Contact> contacts = [];
            var serialSwitchers = TryFindNextSerialSwitchers(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
                usedSwitchers.Add(sw);
            }


            var box1 = new ContactBox(ContactBoxSort.Serial);
            box1.FirstPin = new RelayPlusPin(relay.Name);
            box1.SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes);
            box1.Contacts = contacts;
            boxes.Add(box1);


            /// Calc route via Second(Minus) contact of Relay
            startConnector = relay.Connectors.Skip(1).Single();
            contacts = [];
            serialSwitchers = TryFindNextSerialSwitchers(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
                usedSwitchers.Add(sw);
            }

            var box2 = new ContactBox(ContactBoxSort.Serial);
            startConnector = relay.Connectors.Skip(1).Single();
            box2.FirstPin = new RelayMinusPin(relay.Name);
            box2.SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes);
            box2.Contacts = contacts;
            boxes.Add(box2);
            
        }

        return (boxes, usedSwitchers);
    }

    private static (List<ContactBox> boxes, List<UiSwitcher> switchers) CreateBoxesFromNode(List<UiSwitcher> connectedSwitchers, UiSchemeModel model, List<Node> nodes)
    {
        List<ContactBox> boxes = [];
        List<UiSwitcher> usedSwitchers = [];
        ////From positive pole
        foreach (var node in nodes)
        {
            foreach (var startConnector in node.Connectors)
            {
                List<Contact> contacts = [];
                var serialSwitchers = TryFindNextSerialSwitchers(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
                foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
                {
                    var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                    contacts.Add(new Contact(sw, defState));
                    usedSwitchers.Add(sw);
                }

                var box = new ContactBox(ContactBoxSort.Serial);
                box.FirstPin = node;
                box.SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes);
                box.Contacts = contacts;
                boxes.Add(box);
            }
        }

        return (boxes, usedSwitchers);
    }


    static IEnumerable<(UiSwitcher sw, bool viaOpen, UiConnector connector)> TryFindNextSerialSwitchers(UiConnector startConnector, List<UiSwitcher> switchers, List<UiBinder> binders, List<Node> nodes)
    {
        UiConnector connector = startConnector;

        while (true) 
        {

            var node = nodes.Find(n => n.Connectors.Contains(connector));
            if (node is not null) yield break; //// detect the end of serial chain.

            var binderId = connector.JointBindersId.Single();
            var binder = binders.Single(b => b.Id == binderId);
            var nextConnectorId = binder.StartConnectorId == connector.Id
                ? binder.EndConnectorId
                : binder.StartConnectorId;

            var switcher = switchers.Find(el => el.Connectors.Any(c => c.Id == nextConnectorId));
            if (switcher is null) yield break; /// it mean we have connection with another non-node element
            UiConnector switcherConnector = switcher.Connectors.Single(c => c.Id == nextConnectorId);

            if (switcher.HasBothContacts() && switcherConnector.IsCommon()) yield break;  /// Common connector is like Node, thus, we should cut the chain

            UiConnector? nextConnector =
                switcher.HasOpenContact() && switcherConnector.IsOpen() ? switcher.Common()
                : switcher.HasOpenContact() && switcherConnector.IsCommon() ? switcher.Open()
                : switcher.HasCloseContact() && switcherConnector.IsClose() ? switcher.Common() 
                : switcher.HasCloseContact() && switcherConnector.IsCommon() ? switcher.Close() 
                : null;
 
            if (nextConnector is not null)
            {
                connector = nextConnector!;
                
                yield return (switcher, switcher.HasOpenContact(), connector);
            }

            if(nextConnector is not null && switcher.HasBothContacts() && switcherConnector.IsCommon()) /// Common connector is like Node, thus, we should cut the chain
                yield break;

            
        }

    }

    static ILogicEdge GetLogicEdge(UiConnector connector, UiSchemeModel model, List<Node> nodes)
    {
        var node = nodes.Find(n => n.Connectors.Contains(connector));
        if (node is not null) return node;

        var (el, con) = connector.NextElementAndConnector(model);

        if (el is UiRelay relay)
        {
            return con.Name == ConnectorName.RelPlus
                ? new RelayPlusPin(relay.Name)
                : new RelayMinusPin(relay.Name);
        }

        if (el is UiPole pole)
        {
            return con.Name == ConnectorName.Positive
                ? new PolePositive()
                : new PoleNegative();
        }


        throw new Exception($"Logic edge is not defined for connector {connector.Id}");
    }
}
