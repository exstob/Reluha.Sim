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

public class LogicBox
{
    public LogicBox(LogicBoxType boxType)
    {
        BoxType = boxType;
    }

    public LogicBox(List<LogicBox> boxes)
    {
        if (boxes == null || !boxes.Any())
            throw new ArgumentException("boxes cannot be null or empty");

        BoxType = LogicBoxType.Serial;
        this.Boxes = boxes;
        FirstPin = boxes.First().FirstPin;
        SecondPin = boxes.Last().SecondPin;
    }

    public LogicBoxType BoxType { get; init; }
    public List<Contact> Contacts { get; set; } = [];
    public List<LogicBox> Boxes { get; set; } = [];

    public ILogicEdge FirstPin { get; set; }
    public ILogicEdge SecondPin { get; set; }

    public void Add(Contact contact) => Contacts.Add(contact);
    public List<ILogicEdge> Pins() => [FirstPin, SecondPin];
    public List<Node> Nodes() => FirstPin is Node node1 && SecondPin is Node node2 ? [node1, node2] : [];

    bool IsRoot() => Boxes.Count > 0;
    
    ///// GENERATE LOGIC LIKE: (a & b) | (c & d)
    public override string ToString()
    {
        //string operation = BoxType == LogicBoxType.Serial ? "&" : "|";


        string operation = BoxType == LogicBoxType.Serial ? "&" : "|";

        string? pole = FirstPin is PolePositive ? "Plus" 
                     : FirstPin is PoleNegative ? "Minus"
                     : null;
        string? poleSing = Contacts.Count > 0 && pole is not null ? " & " : null;
        bool hasBoxBrackets = (Contacts.Count == 1 && pole is not null) || Contacts.Count > 1;
        string? leftBracket = hasBoxBrackets ? "(" : null;
        string? rightBracket = hasBoxBrackets ? ")" : null;

        return IsRoot()
            ? $"({string.Join($" {operation} ", this.Boxes)})"
            : $"{leftBracket}{pole}{poleSing}{string.Join($" {operation} ", Contacts)}{rightBracket}";
    }
}


