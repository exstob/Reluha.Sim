using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Sim.Domain;

namespace Sim.Tests
{
    public class ContactStateTest
    {
        [Fact]
        public void OrOperation_with_Chain_Values_Ok()
        {
            var x1 = new ContactState(ContactValue.T);
            var x2 = new ContactState(ContactValue.F);
            //var x2 = ContactValue.F;
            var result = x1 | x2;
            result.ShouldBe(ContactValue.T);

            var x3 = new ContactState(ContactValue.F);
            var x4 = new ContactState(ContactValue.F);
            var x5 = ContactValue.F;
            result = x3 | x4 | x5;
            result.ShouldBe(ContactValue.F);
        }

        [Fact]
        public void AndOperation_with_Chain_and_Contact_Values_Ok()
        {
            var x1 = new ContactState(ContactValue.T);
            var x2 = new ContactState(ContactValue.F);
            var result = x1 & x2;
            result.ShouldBe(ContactValue.F);

            var x3 = new ContactState(ContactValue.T);
            var x4 = ContactValue.T;
            var x5 = ContactValue.T;

            result = x3 & x4 & x5;
            result.ShouldBe(ContactValue.T);
        }
    }
}
