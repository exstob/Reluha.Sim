using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.CompressedScheme;
public enum ContactType
{
    Normal,
    Polar,
    NormalAndOpenPolar, // serial connection of the Nornal and Polar plus contact         ___  --<
    NormalAndClosePolar // serial connection of the Nornal and Polar minus contact           \
}

public enum ContactDefaultState
{
    Open, // Normally open contact - NO (plus state for Polar contact)
    Close // Normally close contact - NC (minus state for Polar contact)
}

public record ContactOptions(ContactDefaultState DefaultState, ContactType Type, bool IsVirtual); 

public class Contact
{
    public string Name { get; init; }
    public ContactOptions Options { get; init; }

    public ContactState State { get; init; }

    //public Pin FirstPin { get; init; }
    //public Pin SecondPin { get; init; }

    public Contact(UiSwitcher switcher, ContactDefaultState defaultState)
    {
        Name = switcher.Name;

        Options  = new ContactOptions
        (
            DefaultState : defaultState,
            Type : Enum.Parse<ContactType>((switcher.ExtraProps as UiSwitcherExtraProps)!.Style),
            IsVirtual : (switcher.ExtraProps as UiSwitcherExtraProps)!.Virtual
        );

        State = (bool)switcher.LogicState;
    }


    //Contact(string name, ContactDefaultState type, Pin coupledPin1, Pin coupledPin2)
    //{
    //    Name = name;
    //    Type = type;
    //    FirstPin = new Pin { Parent = this, Name = name + ".1", СoupledPin = coupledPin1};
    //    SecondPin = new Pin { Parent = this, Name = name + ".2", СoupledPin = coupledPin2 };
    //}

    //Contact(string name, ContactDefaultState type, Pin coupledPin1, Node coupledNode2)
    //{
    //    Name = name;
    //    Type = type;
    //    FirstPin = new Pin { Parent = this, Name = name + ".1", СoupledPin = coupledPin1 };
    //    SecondPin = new Pin { Parent = this, Name = name + ".2", СoupledNode = coupledNode2 };
    //}

    //Contact(string name, ContactDefaultState type, Node coupledNode1, Pin coupledPin2)
    //{
    //    Name = name;
    //    Type = type;
    //    FirstPin = new Pin {Parent = this, Name = name + ".1", СoupledNode = coupledNode1 };
    //    SecondPin = new Pin {Parent = this, Name = name + ".2", СoupledPin = coupledPin2 };
    //}

    //public Contact? NextSerialContact() 
    //{
    //    /// Read the contact chain from left to right (1 -> 2)
    //    if (SecondPin.Parent is Contact contact)
    //        return contact;
    //    else
    //        return null;
    //}
}
