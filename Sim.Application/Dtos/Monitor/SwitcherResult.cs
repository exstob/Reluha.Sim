using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.Dtos.Monitor;

public class SwitcherResult
{
    public required string Name { get; set; }
    public string? Id { get; set; }
    public bool NormalContact { get; init; }
    public bool PolarContact { get; init; }
}
