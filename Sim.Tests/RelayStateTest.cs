using Sim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;


namespace Sim.Tests
{
    public class RelayStateTest
    {
        [Fact]
        public async Task OrOperation_with_Chain_Values_Ok()
        {
            var relay = new RelayState("PP + A & B", "A | B");

            var x1 = new ChainState(ChainValue.P);
            var x2 = new ChainState(ChainValue.N);

            var result = await relay.Calc(x1, x2);

            result.Value.ShouldBe(ChainValue.Z);


            var x3 = new ChainState(ChainValue.P);
            var x4 = new ChainState(ChainValue.P);

            result = await relay.Calc(x3, x4);

            result.Value.ShouldBe(ChainValue.P);

        }

    }
}
