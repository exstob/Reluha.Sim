using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using Sim.Domain.UiSchematic.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.NanoServices;

public static class Convertor
{
    //private static StringBuilder _andAccumulator = new StringBuilder();

    public static SchemeLogicModel ToLogicModel(UiSchemeModel elements)
    {
        var allElements = elements.AllElements();
        var binders = elements.Binders;
        var model = new SchemeLogicModel(Guid.NewGuid());
        Dictionary<string, bool> _andAccumulator = [];

        var relayPlusIn = "";
        var relayMinusIn = "";


        bool isRoot = false; /// indicator, that we have the parallel connection

        foreach (var relay in elements.Relays)
        {
            foreach (var connector in relay.Connectors)  // RelPlus and minus connector
            {
                if (connector.JointBindersId.Count == 0)
                {
                    Console.WriteLine($"Error: No connections for connector {connector.JointBindersId}");
                    continue;
                }

                isRoot = connector.JointBindersId.Count >= 1; 

                foreach (var binder in connector.JointBindersId)
                {
                    var nextElement = FindNextElement(binder, connector.Id, allElements, binders);
                    if (nextElement.ClassName == ElementClassName.Switcher)
                    {
                        var state = nextElement.LogicState as ContactState;
                        _andAccumulator.Add(nextElement.Name, state!);
                    }
                    else if (nextElement.ClassName == ElementClassName.Pole)
                    {
                        if (nextElement.Connectors.Single().Name == ConnectorName.Positive)
                        {

                        }
                        else /// negative
                        {

                        }
          
                    }

                }


            }
        }


        return model;
    }


    static UiElement FindNextElement(string binderId, string connectorId, List<UiElement> elements, List<UiBinder> binders)
    {
        var binder = binders.Single(b => b.Id == binderId);
        var nextConnector = binder.StartConnectorId == connectorId
            ? binder.EndConnectorId
            : binder.StartConnectorId;

        return elements.Single(el => el.Connectors.Any(c => c.Id == nextConnector));
    }

    //private static List<UiElement> AllElements(this UiSchemeModel elements)
    //{
    //    List<UiElement> allElements = [];
    //    allElements.AddRange(elements.Relays);
    //    allElements.AddRange(elements.Switchers);
    //    allElements.AddRange(elements.NegPoles);
    //    allElements.AddRange(elements.PosPoles);
    //    return allElements;
    //}

    private static string ToLogicString(this Dictionary<string, bool> accumulator, string op = "&" )
    {
        var accum = accumulator.Select(a => a.Value ? a.Key : $"!{a.Key}");
        return string.Join($" {op} ", accum); 
    }



}

