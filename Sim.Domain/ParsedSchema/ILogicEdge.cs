using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.ParsedScheme;

public interface ILogicEdge
{
}

public interface IPoleEdge : ILogicEdge
{
}

public interface IRelayEdge : ILogicEdge
{
}

public class RelayPlusPin(string name) : IRelayEdge
{
    public string RelayName { get; } =  name;
}

public class RelayMinusPin(string name) : IRelayEdge
{
    public string RelayName { get; } = name;
}

public class PolePositive : IPoleEdge
{
}

public class PoleNegative : IPoleEdge
{
}