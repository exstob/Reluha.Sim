using System;

namespace Sim.Domain.Logic;

public enum ContactValue
{
    F, // false
    T, // true
}


public class ContactState
{

    public ContactValue Value { get; set; }

    private ContactState()
    {
        Value = ContactValue.F;
    }
    public ContactState(ContactValue v)
    {
        Value = v;
    }

    public static implicit operator ContactValue(ContactState state)
    {
        return state.Value;
    }

    public static implicit operator ContactState(ContactValue value)
    {
        return new ContactState(value);
    }

    //public static explicit operator ContactState(ContactValue value)
    //{
    //    return new ContactState(value);
    //}

    public override string ToString() => $"{Value}";

    public static ContactState operator &(ContactState lhs, ContactState rhs)  // apply where different poles are connected
    {
        return (lhs.Value, rhs.Value) switch
        {
            (ContactValue.F, ContactValue.F) => new ContactState(ContactValue.F),
            (ContactValue.F, ContactValue.T) => new ContactState(ContactValue.F),
            (ContactValue.T, ContactValue.F) => new ContactState(ContactValue.F),
            (ContactValue.T, ContactValue.T) => new ContactState(ContactValue.T),

            _ => (ContactState)ContactValue.F
        };
    }

    public static ContactState operator |(ContactState lhs, ContactState rhs)  // apply where different poles are connected
    {
        return (lhs.Value, rhs.Value) switch
        {
            (ContactValue.F, ContactValue.F) => new ContactState(ContactValue.F),
            (ContactValue.F, ContactValue.T) => new ContactState(ContactValue.T),
            (ContactValue.T, ContactValue.F) => new ContactState(ContactValue.T),
            (ContactValue.T, ContactValue.T) => new ContactState(ContactValue.T),

            _ => (ContactState)ContactValue.F
        };
    }


    public static ContactState operator !(ContactState state)
    {
        return state.Value switch
        {
            ContactValue.F => new ContactState(ContactValue.T),
            ContactValue.T => new ContactState(ContactValue.F),

            _ => new ContactState(ContactValue.F)
        };
    }

}
