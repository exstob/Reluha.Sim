using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.ParsedScheme;

public enum ContactBoxType
{
    Serial,
    Parallel
}

public class ContactBox(ContactBoxType boxType) 
{
    public ContactBoxType BoxType { get; init; } = boxType;
    public List<Contact> Contacts { get; set; } = [];
    public List<ContactBox> Boxes { get; set; } = [];

    public required ILogicEdge FirstPin { get; set; }
    public required ILogicEdge SecondPin { get; set; }

    public void Add(Contact contact) => Contacts.Add(contact);

    bool IsRoot() => Boxes.Count > 0;
    
    ///// GENERATE LOGIC LIKE: (a & b) | (c & d)
    public override string ToString()
    {
        string operation = BoxType == ContactBoxType.Serial ? "&" : "|";

        return IsRoot()
            ? $"( {string.Join($" {operation} ", this.Boxes)} )"
            : $"( {string.Join($" {operation} ", Contacts)} )";
    }
}


