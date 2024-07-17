using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Sim.Domain.Logic;

namespace Sim.Tests
{
    public class ChainResultTest
    {
        [Fact]
        public void OrOperation_with_Chain_Values_Ok()
        {
            var x1 = new ChainState(ChainValue.P);
            var x2 = new ChainState(ChainValue.N);
            var result = x1 | x2;
            result.Value.ShouldBe(ChainValue.C);

            var x3 = new ChainState(ChainValue.N);
            var x4 = new ChainState(ChainValue.Z);
            var x5 = new ChainState(ChainValue.N);
            result = x3 | x4 | x5;
            result.Value.ShouldBe(ChainValue.N);
        }

        [Fact]
        public void AndOperation_with_Chain_Values_Ok()
        {
            var x1 = new ChainState(ChainValue.P);
            var x2 = new ChainState(ChainValue.P);
            var result = x1 & x2;
            result.Value.ShouldBe(ChainValue.P);

            x1 = new ChainState(ChainValue.N);
            x2 = new ChainState(ChainValue.N);
            result = x1 & x2;
            result.Value.ShouldBe(ChainValue.N);

            x1 = new ChainState(ChainValue.N);
            x2 = new ChainState(ChainValue.C);
            result = x1 & x2;
            result.Value.ShouldBe(ChainValue.Z);
        }

        [Fact]
        public void XorOperation_with_Chain_Values_Ok()
        {
            var x1 = new ChainState(ChainValue.P);
            var x2 = new ChainState(ChainValue.N);
            var result = x1 ^ x2;
            result.Value.ShouldBe(ChainValue.P);

            x1 = new ChainState(ChainValue.N);
            x2 = new ChainState(ChainValue.P);
            result = x1 ^ x2;
            result.Value.ShouldBe(ChainValue.N);

            x1 = new ChainState(ChainValue.P);
            x2 = new ChainState(ChainValue.P);
            result = x1 ^ x2;
            result.Value.ShouldBe(ChainValue.Z);
        }

        [Fact]
        public void NotOperation_with_Chain_Values_Ok()
        {
            var x1 = new ChainState(ChainValue.P);
            var result = !x1;
            result.Value.ShouldBe(ChainValue.N);

            x1 = new ChainState(ChainValue.N);
            result = !x1;
            result.Value.ShouldBe(ChainValue.P);

        }

        [Fact]
        public void ImplOperation_with_Chain_Values_Ok()
        {
            var x1 = new ChainState(ChainValue.P);
            var x2 = new ChainState(ChainValue.N);

            var result = (ChainValue.P & x1) & !(ChainValue.N & x2);
            result.Value.ShouldBe(ChainValue.P);

            x1 = new ChainState(ChainValue.N);
            x2 = new ChainState(ChainValue.P);
            result = (ChainValue.P & x1) & !(ChainValue.N & x2);
            result.Value.ShouldBe(ChainValue.Z);


            x1 = new ChainState(ChainValue.P);
            x2 = new ChainState(ChainValue.N);

            result = (ChainValue.P & x1) ^ x2;
            result.Value.ShouldBe(ChainValue.P);
        }

        [Fact]
        public void AndOperation_with_Chain_and_Contact_Values_Ok()
        {
            var x1 = new ChainState(ChainValue.P);
            var x2 = ContactValue.F;
            var result = x1 & x2;
            result.Value.ShouldBe(ChainValue.Z);


            result = x2 & x1;
            result.Value.ShouldBe(ChainValue.Z);
        }
    }
}
