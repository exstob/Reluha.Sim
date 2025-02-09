using Sim.Application.Dtos.Simulate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.Dtos.Monitor;

public class MonitorResult
{
    public required string SchemeId { get; set; }
    public RelayResult? Relay { get; set; }
    public SwitcherResult? Switcher { get; set; }

}
