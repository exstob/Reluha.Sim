using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain
{
    internal class LogicOperation
    {
        public LogicOperation() { }
        public ChainValue ChainAnd(params string[] ops) 
        {
            return ChainValue.U;
        }  
    }
}
