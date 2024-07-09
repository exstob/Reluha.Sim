using Sim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application
{
    internal interface ISimModel
    {
        internal void Create(SchemeElements elements);
    }
}
