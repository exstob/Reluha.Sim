using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Sim.Domain.ParsedScheme;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain.Logic
{
    public class LogicModel(List<Relay> relays, List<Contact> contacts)
    {
        public Ulid Id { get; } = Ulid.NewUlid();
        private readonly InputContactGroupDto _contactGroups = InitContacts(contacts);
        public readonly List<Relay> Relays = relays;
        private ScriptState? script;

        private static InputContactGroupDto InitContacts(List<Contact> contacts)
        {
            InputContactGroupDto contactGroups = new();
            foreach (var contact in contacts)
            {
                var expandDict = contactGroups.x as IDictionary<string, object>;
                var contactName = contact.FullName();
                if (!expandDict!.ContainsKey(contactName))
                    expandDict.Add(contactName, contact.State);
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

        public async Task Compile() 
        {
            script ??= await GenerateScript();
        } 

        private async Task<ScriptState> GenerateScript()
        {
            string modelScript = string.Join(Environment.NewLine, Relays.Select(r => $"public ChainState {r.Name}() => {r.State.ToLogic()};"));

            var scriptOptions = ScriptOptions.Default
                .AddReferences(typeof(ChainState).Assembly)
                .AddReferences(typeof(ContactState).Assembly)
                .AddReferences(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly)
                .AddImports("System")
                .AddImports("Sim.Domain.Logic");

            var _inputScript = CSharpScript.Create(modelScript, scriptOptions, typeof(InputContactGroupDto));
            _inputScript.Compile();

            var scriptState = await _inputScript.RunAsync(_contactGroups, new CancellationToken());
            return scriptState;

        }

        public async Task<(bool, List<Relay>)> Evaluate()
        {
            //var tasks = Relays.Select(r => Task.Run(() => r.State.Calc(_contactGroups))).ToList();
            // var tasks = Relays.Select(r => r.State.Calc(_contactGroups)).ToList();
            var tasks = Relays.Select(r => r.State.CalcFromExternal(_contactGroups, script, $"{r.Name}()")).ToList();
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
            await Compile();
            List<Relay> relays = [];
            bool loop = true;

            while (loop)
            {
                var (isUpdated, updatedRelays) = await this.Evaluate().ConfigureAwait(false);
                relays.AddRange(updatedRelays);
                loop = isUpdated;
                Console.WriteLine("Updated relays: " + string.Join(", ", updatedRelays));
            }

            return relays;
        }
    }
}
