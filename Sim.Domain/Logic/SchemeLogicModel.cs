using Sim.Domain.CompressedScheme;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.Logic
{
    public class SchemeLogicModel(Guid id)
    {
        public Guid Id { get; set; } = id;
        private InputContactGroupDto contactGroups = new();
        private List<Relay> relays = [];

  
        public void UpdateContact(string contactName, ContactState value, ContactGroupsEnum groupName = ContactGroupsEnum.Virtual)
        {
            var expandoDict = groupName switch
            {
                ContactGroupsEnum.Virtual => contactGroups.v as IDictionary<string, object>,
                ContactGroupsEnum.Polar => contactGroups.p as IDictionary<string, object>,
                ContactGroupsEnum.Normal => contactGroups.n as IDictionary<string, object>,
                _ => throw new ArgumentException("Unknown Group of contact", groupName.ToString())
            }; 


            if (expandoDict!.ContainsKey(contactName))
                expandoDict[contactName] = value;
            else
                expandoDict.Add(contactName, value);
        }

        public ContactState GetContact(string contactName, ContactGroupsEnum groupName = ContactGroupsEnum.Virtual)
        {
            var expandoDict = groupName switch
            {
                ContactGroupsEnum.Virtual => contactGroups.v as IDictionary<string, object>,
                ContactGroupsEnum.Polar => contactGroups.p as IDictionary<string, object>,
                ContactGroupsEnum.Normal => contactGroups.n as IDictionary<string, object>,
                _ => throw new ArgumentException("Unknown Group of contact", groupName.ToString())
            };
            return (ContactState)expandoDict[contactName];
        }

        public void AddRelay(Relay relay)
        {
            relays.Add(relay);
        }

        public async Task<bool> Evaluate()
        {
            //foreach (var rel in relays) 
            //{
            //    rel.Calc(contactGroups);
            //}

            //Parallel.ForEach(relays, state => state.Calc(contactGroups));

            var tasks = relays.Select(r => Task.Run(() => r.State.Calc(contactGroups))).ToList();
            await Task.WhenAll(tasks);

            var isUpdated = false;
            foreach (var relay in relays) 
            {
                if (relay.State.isUpdated)
                {
                    UpdateContact(relay.Name, relay.State.NormalContact, ContactGroupsEnum.Normal);
                    UpdateContact(relay.Name, relay.State.PolarContact, ContactGroupsEnum.Polar);
                }

                isUpdated |= relay.State.isUpdated;
            }

            return isUpdated;

        }
    }
}
