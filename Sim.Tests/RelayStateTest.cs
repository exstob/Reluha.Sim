using Sim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using System.Dynamic;
using Sim.Domain.Logic;


namespace Sim.Tests
{
    public class RelayStateTest
    {
        [Fact]
        public async Task OrOperation_with_Chain_Values_Ok()
        {
            var relay = new RelayState("v.PP & v.A & v.B", "(v.C | v.D) & v.NN");

            var x1 = new ChainState(ChainValue.P);
            var x2 = new ChainState(ChainValue.N);

            
            dynamic contactList = new ExpandoObject();

            //var contactList = new InputContactGroupDto();

            contactList.PP = new ChainState(ChainValue.P);
            contactList.A = new ContactState(ContactValue.T);
            contactList.B = new ContactState(ContactValue.T);

            contactList.NN = new ChainState(ChainValue.N);
            contactList.C = new ContactState(ContactValue.F);
            contactList.D = new ContactState(ContactValue.T);

            var wrapContactList = new InputContactGroupDto { v = contactList };

            var result = await relay.Calc(wrapContactList);

            result.NormalContact.Value.ShouldBe(ContactValue.T);
            result.PolarContact.Value.ShouldBe(ContactValue.F);

            result = await relay.Calc(wrapContactList);
            result.NormalContact.Value.ShouldBe(ContactValue.T);
            result.PolarContact.Value.ShouldBe(ContactValue.F);

        }

    }
}
