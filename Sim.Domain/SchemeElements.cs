using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain;

public class SchemeElements
{
    public List<Element> Relays { get; set; } = [];
    public List<Element> Switchers { get; set; } = [];
    public List<Element> PosPoles { get; set; } = [];
    public List<Element> NegPoles { get; set; } = [];
    public List<Binder> Binders { get; set; } = new List<Binder>();
}

