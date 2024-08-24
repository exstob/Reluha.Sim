using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.ParsedScheme;

public enum LogicBoxType
{
    Serial,
    Parallel
}

public class LogicBox(LogicBoxType boxType) 
{
    public LogicBoxType BoxType { get; init; } = boxType;
    public List<Contact> Contacts { get; set; } = [];
    public List<LogicBox> Boxes { get; set; } = [];

    public required ILogicEdge FirstPin { get; set; }
    public required ILogicEdge SecondPin { get; set; }

    public void Add(Contact contact) => Contacts.Add(contact);

    bool IsRoot() => Boxes.Count > 0;
    
    ///// GENERATE LOGIC LIKE: (a & b) | (c & d)
    public override string ToString()
    {
        string operation = BoxType == LogicBoxType.Serial ? "&" : "|";
        string? pole = FirstPin is PolePositive ? "Plus" 
                     : FirstPin is PoleNegative ? "Minus"
                     : null;
        string? poleSing = Contacts.Count > 0 ? " & " : null;

        return IsRoot()
            ? $"({string.Join($" {operation} ", this.Boxes)} )"
            : $"({pole}{poleSing}{string.Join($" {operation} ", Contacts)} )";
    }
}


