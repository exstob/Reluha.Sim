using System;

namespace Sim.Domain;

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

    public static implicit operator ContactValue(ContactState contactResult)
    {
        return contactResult.Value;
    }

    public override string ToString() => $"{Value}";

    public static ContactValue operator &(ContactState lhs, ContactState rhs)  // apply where different poles are connected
    {
        return (lhs.Value, rhs.Value) switch
        {
            (ContactValue.F, ContactValue.F) => new ContactState(ContactValue.F),
            (ContactValue.F, ContactValue.T) => new ContactState(ContactValue.F),
            (ContactValue.T, ContactValue.F) => new ContactState(ContactValue.F),
            (ContactValue.T, ContactValue.T) => new ContactState(ContactValue.T),

            _ => ContactValue.F
        };
    }

    //public static ContactValue operator &(ContactState lhs, ContactValue rhs)  // apply where different poles are connected
    //{
    //    return (lhs.Value, rhs) switch
    //    {
    //        (ContactValue.F, ContactValue.F) => new ContactState(ContactValue.F),
    //        (ContactValue.F, ContactValue.T) => new ContactState(ContactValue.F),
    //        (ContactValue.T, ContactValue.F) => new ContactState(ContactValue.F),
    //        (ContactValue.T, ContactValue.T) => new ContactState(ContactValue.T),

    //        _ => ContactValue.F
    //    };
    //}

    public static ContactValue operator |(ContactState lhs, ContactState rhs)  // apply where different poles are connected
    {
        return (lhs.Value, rhs.Value) switch
        {
            (ContactValue.F, ContactValue.F) => new ContactState(ContactValue.F),
            (ContactValue.F, ContactValue.T) => new ContactState(ContactValue.T),
            (ContactValue.T, ContactValue.F) => new ContactState(ContactValue.T),
            (ContactValue.T, ContactValue.T) => new ContactState(ContactValue.T),

            _ => ContactValue.F
        };
    }


    //public static ContactValue operator |(ContactState lhs, ContactValue rhs)  // apply where different poles are connected
    //{
    //    return (lhs.Value, rhs) switch
    //    {
    //        (ContactValue.F, ContactValue.F) => new ContactState(ContactValue.F),
    //        (ContactValue.F, ContactValue.T) => new ContactState(ContactValue.T),
    //        (ContactValue.T, ContactValue.F) => new ContactState(ContactValue.T),
    //        (ContactValue.T, ContactValue.T) => new ContactState(ContactValue.T),

    //        _ => ContactValue.F
    //    };
    //}

    public static ContactValue operator !(ContactState state)
    {
        return state.Value switch
        {
            (ContactValue.F) => new ContactState(ContactValue.T),
            (ContactValue.T) => new ContactState(ContactValue.F),

            _ => ContactValue.F
        };
    }

}
