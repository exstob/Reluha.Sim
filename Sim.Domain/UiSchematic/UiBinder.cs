using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.UiSchematic;

public class UiBinder
{
    public string Id { get; set; }
    public bool Connected { get; set; } = false;
    public Point StartPoint { get; set; }
    public string StartConnectorId { get; set; }
    public Point EndPoint { get; set; }
    public string EndConnectorId { get; set; } = null;
    public string FriendBinderId { get; set; } = null;
}

