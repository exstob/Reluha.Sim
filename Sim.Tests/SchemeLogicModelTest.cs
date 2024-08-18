using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Sim.Domain.Logic;

namespace Sim.Tests
{
    public class SchemeLogicModelTest
    {
        [Fact]
        public void AddProperty_Ok()
        {
            var model = new SchemeLogicModel(Guid.NewGuid());
            model.UpdateContact("Rel123", new ContactState(ContactValue.T));

            var Rel123 = model.GetContact("Rel123");

            var result_rel = Rel123 as ContactState;
            result_rel?.Value.ShouldBe(ContactValue.T);


            model.UpdateContact("conact_666", new ContactState(ContactValue.T));

            var result_cont = model.GetContact("conact_666") as ContactState;
            result_cont.Value.ShouldBe(ContactValue.T);

        }

        [Fact]
        public async void Evaluate_SchemeLogicModel_Ok()
        {
            var model = new SchemeLogicModel(Guid.NewGuid());

            ///Virtual Contacts
            model.UpdateContact("v1", ContactState.T());
            model.UpdateContact("v2", ContactState.T());
            model.UpdateContact("v3", ContactState.F());
            model.UpdateContact("v4", ContactState.T());

            ///Default Relay Contacts
            model.UpdateContact("R1", ContactState.F(), ContactGroupsEnum.Normal);
            model.UpdateContact("R1", ContactState.F(), ContactGroupsEnum.Polar);


            var relay1 = new RelayState("R1", "Plus & v.v1 & v.v2", "!v.v3 & v.v4 & Minus");
            model.AddRelay(relay1);
            
            var relay2 = new RelayState("R2", "Plus & n.R1", "v.v4 & Minus");
            model.AddRelay(relay2);

            var result = await model.Evaluate();
            var r1 = model.GetContact("R1", ContactGroupsEnum.Normal);
            r1.Value.ShouldBe(ContactValue.T);
            result.ShouldBe(true);  ///because R1 is updated

            result = await model.Evaluate();
            var r2 = model.GetContact("R2", ContactGroupsEnum.Normal);
            r2.Value.ShouldBe(ContactValue.T);
            result.ShouldBe(true); ///because R2 is updated

            result = await model.Evaluate();
            result.ShouldBe(false);

            r1 = model.GetContact("R1", ContactGroupsEnum.Normal);
            r2 = model.GetContact("R2", ContactGroupsEnum.Normal);
            r1.Value.ShouldBe(ContactValue.T);
            r2.Value.ShouldBe(ContactValue.T);

        }

    }
}
