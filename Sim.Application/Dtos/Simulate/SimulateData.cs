using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.Dtos.Simulate;

public class SimulateData
{
    public required string SchemeId { get; set; }
    [MinLength(1)]
    public required List<SimulateStep> Steps { get; set; }
}
