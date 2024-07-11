using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Sim.Domain;

namespace Sim.Tests
{
    public class ChainResultTest
    {
        [Fact]
        public void OrOperation_with_Chain_Values_Ok()
        {
            var x1 = new ChainResult(ChainValue.P);
            var x2 = new ChainResult(ChainValue.N);
            var result = x1 | x2;
            result.ShouldBe(ChainValue.U);

            var x3 = new ChainResult(ChainValue.N);
            var x4 = new ChainResult(ChainValue.Z);
            var x5 = new ChainResult(ChainValue.N);
            result = x3 | x4 | x5;
            result.ShouldBe(ChainValue.N);
        }

        [Fact]
        public void AndOperation_with_Chain_and_Contact_Values_Ok()
        {
            var x1 = new ChainResult(ChainValue.P);
            var x2 = ContactValue.F;
            var result = x1 & x2;
            result.ShouldBe(ChainValue.Z);


            result = x2 & x1;
            result.ShouldBe(ChainValue.Z);
        }
    }
}
