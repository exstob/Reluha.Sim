using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.UiSchematic;

public class UiSchemeModel
{
    public List<UiElement> Relays { get; set; } = [];
    public List<UiElement> Switchers { get; set; } = [];
    public List<UiElement> PosPoles { get; set; } = [];
    public List<UiElement> NegPoles { get; set; } = [];
    public List<UiBinder> Binders { get; set; } = new List<UiBinder>();
}

