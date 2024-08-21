using Sim.Domain.ParsedScheme;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.Logic
{
    public class LogicModel(List<Relay> relays, List<Contact> contacts)
    {
        public Guid Id { get; } = Guid.NewGuid();
        private InputContactGroupDto _contactGroups = InitContacts(contacts);
        private List<Relay> _relays = relays;
        //private List<Contact> _contacts = contacts;

        public static InputContactGroupDto InitContacts(List<Contact> contacts)
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
            //var expandDict = groupName switch
            //{
            //    ContactGroupsEnum.Virtual => _contactGroups.v as IDictionary<string, object>,
            //    ContactGroupsEnum.Polar => _contactGroups.p as IDictionary<string, object>,
            //    ContactGroupsEnum.Normal => _contactGroups.n as IDictionary<string, object>,
            //    _ => throw new ArgumentException("Unknown Group of contact", groupName.ToString())
            //};
            var expandDict = _contactGroups.x as IDictionary<string, object>;
            return (ContactState)expandDict![contactName];
        }

        public void AddRelay(Relay relay)
        {
            _relays.Add(relay);
        }

        public async Task<bool> Evaluate()
        {
            var tasks = _relays.Select(r => Task.Run(() => r.State.Calc(_contactGroups))).ToList();
            await Task.WhenAll(tasks);

            var isUpdated = false;
            foreach (var relay in _relays) 
            {
                if (relay.State.IsUpdated)
                {
                    UpdateContact(relay.Name, relay.State.NormalContact);
                    UpdateContact($"{relay.Name}.{nameof(ContactType.Polar)}", relay.State.PolarContact);
                }

                isUpdated |= relay.State.IsUpdated;
            }

            return isUpdated;

        }
    }
}
