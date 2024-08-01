﻿using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.CompressedScheme;

public class CompressedScheme
{
    public List<Relay> Relays { get; set; } = [];
    public List<ContactBox> Contacts { get; set; } = [];
    //public List<UiElement> PosPoles { get; set; } = [];
    //public List<UiElement> NegPoles { get; set; } = [];
    public List<Node> Nodes { get; set; } = [];
}

