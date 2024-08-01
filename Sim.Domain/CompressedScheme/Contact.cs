using Sim.Domain.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.CompressedScheme;
public enum ContactType
{
    Open, // Normally open contact - NO
    Close // Normally close contact - NC
}


public class Contact
{
    public string Name { get; init; }
    public ContactType Type { get; init; }

    public ContactState State { get; init; }

    public Pin FirstPin { get; init; }
    public Pin SecondPin { get; init; }

    Contact(string name, ContactType type, Pin coupledPin1, Pin coupledPin2)
    {
        Name = name;
        Type = type;
        FirstPin = new Pin { Parent = this, Name = name + ".1", СoupledPin = coupledPin1};
        SecondPin = new Pin { Parent = this, Name = name + ".2", СoupledPin = coupledPin2 };
    }

    Contact(string name, ContactType type, Pin coupledPin1, Node coupledNode2)
    {
        Name = name;
        Type = type;
        FirstPin = new Pin { Parent = this, Name = name + ".1", СoupledPin = coupledPin1 };
        SecondPin = new Pin { Parent = this, Name = name + ".2", СoupledNode = coupledNode2 };
    }

    Contact(string name, ContactType type, Node coupledNode1, Pin coupledPin2)
    {
        Name = name;
        Type = type;
        FirstPin = new Pin {Parent = this, Name = name + ".1", СoupledNode = coupledNode1 };
        SecondPin = new Pin {Parent = this, Name = name + ".2", СoupledPin = coupledPin2 };
    }

    public Contact? NextSerialContact() 
    {
        /// Read the contact chain from left to right (1 -> 2)
        if (SecondPin.Parent is Contact contact)
            return contact;
        else
            return null;
    }
}
