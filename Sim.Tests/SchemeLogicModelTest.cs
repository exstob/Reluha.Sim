using Sim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Sim.Domain.Logic;
using Sim.Domain;

namespace Sim.Tests
{
    public class SchemeLogicModelTest
    {
        [Fact]
        public void AddProperty_Ok()
        {
            SchemeLogicModel.AddProperty("Rel123", new ChainState(ChainValue.P));

            var Rel123 = SchemeLogicModel.GetProperty("Rel123");

            var result_rel = Rel123 as ChainState;
            result_rel?.Value.ShouldBe(ChainValue.P);


            SchemeLogicModel.AddProperty("conact_666", new ContactState(ContactValue.T));

            var result_cont = SchemeLogicModel.GetProperty("conact_666") as ContactState;
            result_cont.Value.ShouldBe(ContactValue.T);

        }

    }
}
