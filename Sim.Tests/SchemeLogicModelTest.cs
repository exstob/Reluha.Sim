using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Sim.Domain.Logic;
using Sim.Domain.ParsedScheme;
using Sim.Domain.UiSchematic;

namespace Sim.Tests
{
    public class SchemeLogicModelTest
    {
        [Fact]
        public void AddProperty_Ok()
        {
            List<Relay> relays = []; 
            List<Contact> contacts = [];
            var model = new LogicModel(relays, contacts);
            model.UpdateContact("Rel123", new ContactState(ContactValue.T));

            var Rel123 = model.GetContact("Rel123");

            var result_rel = Rel123 as ContactState;
            result_rel?.Value.ShouldBe(ContactValue.T);


            model.UpdateContact("conact_666", new ContactState(ContactValue.T));

            var result_cont = model.GetContact("conact_666") as ContactState;
            result_cont.Value.ShouldBe(ContactValue.T);

        }

        [Fact]
        public async Task Evaluate_SchemeLogicModel_Ok()
        {
            List<Relay> relays = [
                new Relay { Name = "R1", State = new RelayState("Plus & x.v1 & x.v2", "!x.v3 & x.v4 & Minus") },
                new Relay { Name = "R2", State = new RelayState( "Plus & x.R1", "x.v4 & Minus") },
            ];

            var options = new ContactOptions(ContactDefaultState.Open, ContactType.Normal, true);
            List<Contact> contacts = [
                 new Contact ("v1", options, ContactState.T()),
                 new Contact ("v2", options, ContactState.T()),
                 new Contact ("v3", options, ContactState.F()),
                 new Contact ("v4", options, ContactState.T()),
                 new Contact ("R1", options, ContactState.F()),
                 new Contact ("R1.Polar", options, ContactState.F()),
            ];

            var model = new LogicModel(relays, contacts);
            await model.Compile();

            var (result, _) = await model.Evaluate();
            var r1 = model.GetContact("R1");
            r1.Value.ShouldBe(ContactValue.T);
            result.ShouldBe(true);  ///because R1 is updated

            (result, _) = await model.Evaluate();
            var r2 = model.GetContact("R2");
            r2.Value.ShouldBe(ContactValue.T);
            result.ShouldBe(true); ///because R2 is updated

            (result, _) = await model.Evaluate();
            result.ShouldBe(false);

            r1 = model.GetContact("R1");
            r2 = model.GetContact("R2");
            r1.Value.ShouldBe(ContactValue.T);
            r2.Value.ShouldBe(ContactValue.T);

        }

    }
}
