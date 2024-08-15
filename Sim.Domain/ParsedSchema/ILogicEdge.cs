using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.ParsedScheme;

public interface ILogicEdge
{
}

public class RelayPlusPin(string name) : ILogicEdge
{
    public string RelayName { get; } =  name;
}

public class RelayMinusPin(string name) : ILogicEdge
{
    public string RelayName { get; } = name;
}

public class PolePositive : ILogicEdge
{
}

public class PoleNegative : ILogicEdge
{
}