using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.Logic
{
    /// <summary>
    /// Wrapper for list of difference contacts.
    /// We are forced to use it since CSharpScript do not support the dynamic object directly
    /// </summary>
    public class InputContactGroupDto
    {
        /// <summary>
        /// Variables
        /// </summary>
        public dynamic x { get; set; } = new ExpandoObject();
        public ChainState Plus { get; set; } = ChainState.P();
        public ChainState Minus { get; set; } = ChainState.N();
    }
}
