using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain
{
    internal class RelayState
    {
        public sbyte NormalContact { get; set; }
        public sbyte PolarContact { get; set; }
        public int StartDelay { get; set; }
        public int EndDelay { get; set; }
        public ChainState PositiveInput { get; set; }
        public ChainState NegativeInput { get; set; }

        RelayState() 
        {
            //PositiveInput = ChainValue.U;
            //NegativeInput = ChainValue.U;
        }
    }
}
