using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain
{

    enum ChainValue 
    {
        Z,
        O,
        P,
        N,
     }

    internal class ChainResult
    {
        ChainValue Value { get; set; } = ChainValue.O;
    }
}
