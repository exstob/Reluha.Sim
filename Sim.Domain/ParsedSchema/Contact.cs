using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.ParsedScheme;
public enum ContactType
{
    Normal,
    Polar,
    NormalAndOpenPolar, // serial connection of the Normal and Polar plus contact         ___  --<
    NormalAndClosePolar // serial connection of the Normal and Polar minus contact           \
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

    public string FullName() 
    {
        var prop = Options.Type == ContactType.Normal ? null : $".{Options.Type}";
        return $"{Name}{prop}";
    }

    public override string ToString()
    { 
        var inverse = Options.DefaultState == ContactDefaultState.Open ? null : "!";
        var prop = Options.Type == ContactType.Normal ? null : $".{Options.Type}";
        return $"{inverse}x.{Name}{prop}";
    }
}
