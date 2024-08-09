using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.CompressedScheme;

public enum ContactBoxSort
{
    Serial,
    Parallel
}

public class ContactBox(ContactBoxSort sort) 
{
    public ContactBoxSort Sort { get; init; } = sort;
    public List<Contact> Contacts { get; set; } = [];
    //public Pin FirstPin { get; set; }
    //public Pin SecondPin { get; set; }

    public ILogicEdge FirstPin { get; set; }
    public ILogicEdge SecondPin { get; set; }

    public void Add(Contact contact) => Contacts.Add(contact);
}


