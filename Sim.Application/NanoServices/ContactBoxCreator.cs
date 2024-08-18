﻿using Sim.Domain.ParsedScheme;
using Sim.Domain.UiSchematic;
using Sim.Domain.UiSchematic.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.NanoServices;

public static class ContactBoxCreator
{
    public static List<ContactBox> Create(UiSchemeModel model, List<Node> nodes)
    {
        var connectedSwitchers = model.Switchers.Where(sw => sw.HasCloseContact() || sw.HasOpenContact()).ToList();
        List<ContactBox> boxes = [];
        
        /// Analyze routes from  Pole => Node path
        var (childBoxes, usedSwitchers) = ContactBoxCreator.CreateBoxesStartedFromPole(connectedSwitchers, model, nodes);
        boxes.AddRange(childBoxes);
        connectedSwitchers.RemoveAll(csw => usedSwitchers.Contains(csw));

        /// Analyze routes from CommonConnector contact
         (childBoxes, usedSwitchers) = ContactBoxCreator.CreateBoxesStartedFromCommonContacts(connectedSwitchers, model, nodes);
        boxes.AddRange(childBoxes);
        connectedSwitchers.RemoveAll(csw => usedSwitchers.Contains(csw));

        /// Analyze routes from Node to Node
        (childBoxes, usedSwitchers) = ContactBoxCreator.CreateBoxesStartedFromNode(connectedSwitchers, model, nodes);
        boxes.AddRange(childBoxes);
        connectedSwitchers.RemoveAll(csw => usedSwitchers.Contains(csw));

        /// TODO: It need to find out when it should be necessary to get routes from Relay to another Relay  
        /// Analyze routes from Relay path 
        //(boxes, usedSwitchers) = ContactBoxCreator.CreateBoxesStartedFromRelay(connectedSwitchers, model, nodes);
        //boxes.AddRange(childBoxes);
        //connectedSwitchers.RemoveAll(csw => usedSwitchers.Contains(csw));


        return boxes;
    }

    private static (List<ContactBox> boxes, List<UiSwitcher> switchers) CreateBoxesStartedFromCommonContacts(List<UiSwitcher> connectedSwitchers, UiSchemeModel model, List<Node> nodes)
    {
        List<ContactBox> boxes = [];
        List<UiSwitcher> usedSwitchers = [];
        var fullSwitchers = connectedSwitchers.Where(sw => sw.HasBothContacts());
        foreach (var switcher in fullSwitchers)
        {

            /// Calc route via OpenConnector contact
            var startConnector = switcher.CommonConnector();
            List<Contact> contacts = [new Contact(switcher, ContactDefaultState.Open)];
            var serialSwitchers = TryFindSerialSwitchers(switcher.OpenConnector(), connectedSwitchers, model.Binders, nodes).ToList();
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defaultState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defaultState));
            }


            var box1 = new ContactBox(ContactBoxType.Serial)
            {
                FirstPin = nodes.GetNodeFor(startConnector),
                SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes),
                Contacts = contacts,

            };
            boxes.Add(box1);


            /// Calc route via CloseConnector contact
            contacts = [new Contact(switcher, ContactDefaultState.Close)];
            serialSwitchers = TryFindSerialSwitchers(switcher.CloseConnector(), connectedSwitchers, model.Binders, nodes).ToList();
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
            }

            var box2 = new ContactBox(ContactBoxType.Serial)
            {
                FirstPin = nodes.GetNodeFor(startConnector),
                SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes),
                Contacts = contacts,

            };
            boxes.Add(box2);

            usedSwitchers.Add(switcher);
        }

        return (boxes, usedSwitchers);
    }


    private static (List<ContactBox> boxes, List<UiSwitcher> switchers) CreateBoxesStartedFromPole(List<UiSwitcher> connectedSwitchers, UiSchemeModel model, List<Node> nodes)
    {
        List<ContactBox> boxes = [];
        List<UiSwitcher> usedSwitchers = [];
        ////From positive pole
        foreach (var pole in model.PosPoles.FindAll(p => p.Connectors.FirstOrDefault()?.Connected ?? false))
        {
            var startConnector = pole.Connectors.Single();
            var serialSwitchers = TryFindSerialSwitchers(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
            List<Contact> contacts = [];
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
                usedSwitchers.Add(sw);
            }
            if (contacts.Count == 0) continue;

            var box = new ContactBox(ContactBoxType.Serial)
            {
                FirstPin = new PolePositive(),
                SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes),
                Contacts = contacts
            };

            boxes.Add(box);
        }

        ////From negative pole
        foreach (var pole in model.NegPoles.FindAll(p => p.Connectors.FirstOrDefault()?.Connected ?? false))
        {

            var startConnector = pole.Connectors.Single();
            var serialSwitchers = TryFindSerialSwitchers(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
            List<Contact> contacts = [];
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
                usedSwitchers.Add(sw);
            }
            if (contacts.Count == 0) continue;

            var box = new ContactBox(ContactBoxType.Serial)
            {
                FirstPin = new PoleNegative(),
                SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes),
                Contacts = contacts
            };
            boxes.Add(box);
        }

        return (boxes, usedSwitchers);
    }

    private static (List<ContactBox> boxes, List<UiSwitcher> switchers) CreateBoxesStartedFromRelay(List<UiSwitcher> connectedSwitchers, UiSchemeModel model, List<Node> nodes)
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
            var serialSwitchers = TryFindSerialSwitchers(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
                usedSwitchers.Add(sw);
            }

            var box1 = new ContactBox(ContactBoxType.Serial)
            {
                FirstPin = new RelayPlusPin(relay.Name),
                SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes),
                Contacts = contacts,
            };

            boxes.Add(box1);


            /// Calc route via Second(Minus) contact of Relay
            startConnector = relay.Connectors.Skip(1).Single();
            contacts = [];
            serialSwitchers = TryFindSerialSwitchers(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
            foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
            {
                var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                contacts.Add(new Contact(sw, defState));
                usedSwitchers.Add(sw);
            }

            var box2 = new ContactBox(ContactBoxType.Serial)
            {
                FirstPin = new RelayMinusPin(relay.Name),
                SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes),
                Contacts = contacts,
            };

            boxes.Add(box2);

        }

        return (boxes, usedSwitchers);
    }

    private static (List<ContactBox> boxes, List<UiSwitcher> switchers) CreateBoxesStartedFromNode(List<UiSwitcher> connectedSwitchers, UiSchemeModel model, List<Node> nodes)
    {
        List<ContactBox> boxes = [];
        List<UiSwitcher> usedSwitchers = [];
        ////From positive pole
        foreach (var node in nodes)
        {
            foreach (var startConnector in node.Connectors)
            {
                List<Contact> contacts = [];
                var serialSwitchers = TryFindSerialSwitchers(startConnector, connectedSwitchers, model.Binders, nodes).ToList();
                foreach ((UiSwitcher sw, bool viaOpenContact, _) in serialSwitchers)
                {
                    var defState = viaOpenContact ? ContactDefaultState.Open : ContactDefaultState.Close;
                    contacts.Add(new Contact(sw, defState));
                    usedSwitchers.Add(sw);
                }

                var box = new ContactBox(ContactBoxType.Serial)
                {
                    FirstPin = node,
                    SecondPin = GetLogicEdge(serialSwitchers.Last().connector, model, nodes),
                    Contacts = contacts,
                };

                boxes.Add(box);
            }
        }

        return (boxes, usedSwitchers);
    }


    static IEnumerable<(UiSwitcher sw, bool viaOpen, UiConnector connector)>
        TryFindSerialSwitchers(UiConnector startConnector, List<UiSwitcher> switchers, List<UiBinder> binders, List<Node> nodes)
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

            if (switcher.HasBothContacts() && switcherConnector.IsCommon()) yield break;  /// CommonConnector pin is like Node, thus, we should cut the chain

            UiConnector? nextConnector =
                switcher.HasOpenContact() && switcherConnector.IsOpen() ? switcher.CommonConnector()
                : switcher.HasOpenContact() && switcherConnector.IsCommon() ? switcher.OpenConnector()
                : switcher.HasCloseContact() && switcherConnector.IsClose() ? switcher.CommonConnector()
                : switcher.HasCloseContact() && switcherConnector.IsCommon() ? switcher.CloseConnector()
                : null;

            if (nextConnector is not null)
            {
                connector = nextConnector!;

                yield return (switcher, switcher.HasOpenContact(), connector);
            }

            if (nextConnector is not null && switcher.HasBothContacts() && switcherConnector.IsCommon()) /// CommonConnector pin is like Node, thus, we should cut the chain
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


        throw new Exception($"Logic edge is not defined for pin {connector.Id}");
    }

    private static Node GetNodeFor(this List<Node> nodes, UiConnector connector) => nodes.Find(n => n.Connectors.Contains(connector)) ?? throw new Exception($"No node for {connector.Id}");
}
