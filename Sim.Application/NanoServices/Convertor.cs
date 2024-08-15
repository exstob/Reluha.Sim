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
            
        }


        return model;
    }


    
    private static string ToLogicString(this Dictionary<string, bool> accumulator, string op = "&" )
    {
        var accum = accumulator.Select(a => a.Value ? a.Key : $"!{a.Key}");
        return string.Join($" {op} ", accum); 
    }



}

