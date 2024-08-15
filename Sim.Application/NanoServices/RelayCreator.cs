using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sim.Domain.ParsedScheme;
using Sim.Domain.UiSchematic;

namespace Sim.Application.NanoServices;

public class RelayCreator
{
    public static List<Relay> Create(UiSchemeModel elements, List<ContactBox> boxes)
    {
        List<Relay> relays = [];
        foreach (var relay in elements.Relays)
        {

        }

        return relays;
    }

}
