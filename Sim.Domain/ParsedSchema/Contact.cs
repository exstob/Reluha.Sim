using Microsoft.CodeAnalysis;
using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

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
    public Contact(string name , ContactOptions options, ContactState state)
    {
        Name = name;
        Options = options;
        State = state;
    }
    
    public Contact(UiSwitcher switcher, ContactDefaultState defaultState)
    {
        Name = switcher.Name;

        var props = switcher.ExtraProps;

        Options  = new ContactOptions
        (
            DefaultState : defaultState,
            Type : props?.Style is not null ?  Enum.Parse<ContactType>(props.Style, true) : ContactType.Normal,
            IsVirtual : props?.Virtual ?? true
        );

        State = switcher.LogicState is bool state ? state : false;
    }

    public string FullName() 
    {
        var prop = Options.Type == ContactType.Normal ? null : $".{Options.Type}";
        return $"{Name}{prop}";
    }

    public static string FullName(UiSwitcher switcher)
    {
        var contactType = switcher.ExtraProps?.Style is not null ? Enum.Parse<ContactType>(switcher.ExtraProps.Style, true) : ContactType.Normal;
        var prop = contactType == ContactType.Normal ? null : $".{contactType}";
        return $"{switcher.Name}{prop}";
    }

    public override string ToString()
    { 
        var inverse = Options.DefaultState == ContactDefaultState.Open ? null : "!";
        var prop = Options.Type == ContactType.Normal ? null : $".{Options.Type}";
        return $"{inverse}x.{Name}{prop}";
    }
}
