﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.UiSchematic;

public record UiRelay : UiElement
{
    public new UiRelayExtraProps ExtraProps { get; set; }
}
