using Sim.Domain.ParsedScheme;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace Sim.Domain.Logic
{
    public class LogicModel(List<Relay> relays, List<Contact> contacts)
    {
        public Ulid Id { get; } = Ulid.NewUlid();
        private readonly InputContactGroupDto _contactGroups = InitContacts(contacts);
        public readonly List<Relay> Relays = relays;
        //private List<Contact> _contacts = contacts;

        private static InputContactGroupDto InitContacts(List<Contact> contacts)
        {
            InputContactGroupDto contactGroups = new();
            foreach (var contact in contacts)
            {
                var expandDict = contactGroups.x as IDictionary<string, object>;
                expandDict!.Add(contact.FullName(), contact.State);
            }
            return contactGroups;
        }

        public void UpdateContact(string contactName, ContactState value)
        {
            var expandDict = _contactGroups.x as IDictionary<string, object>;
            if (expandDict!.ContainsKey(contactName))
                expandDict[contactName] = value;
            else
                expandDict.Add(contactName, value);
        }

        public ContactState GetContact(string contactName, ContactType type = ContactType.Normal)
        {
            var expandDict = _contactGroups.x as IDictionary<string, object>;
            return (ContactState)expandDict![contactName];
        }

        public async Task<(bool, List<Relay>)> Evaluate()
        {
            var tasks = Relays.Select(r => Task.Run(() => r.State.Calc(_contactGroups))).ToList();
            await Task.WhenAll(tasks);

            var isUpdated = false;
            List<Relay> updatedRelays = [];
            foreach (var relay in Relays) 
            {
                if (relay.State.IsUpdated)
                {
                    UpdateContact(relay.Name, relay.State.NormalContact);
                    UpdateContact($"{relay.Name}.{nameof(ContactType.Polar)}", relay.State.PolarContact);
                    updatedRelays.Add(relay);
                }

                isUpdated |= relay.State.IsUpdated;
            }

            return (isUpdated, updatedRelays);

        }

        public async Task<List<Relay>> EvaluateAll()
        {
            List<Relay> relays = [];
            bool loop = true;

            while (loop)
            {
                var (isUpdated, updatedRelays) = await this.Evaluate();
                relays.AddRange(updatedRelays);
                loop = isUpdated;
            }

            return relays;
        }
    }
}
