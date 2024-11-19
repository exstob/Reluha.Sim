using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Diagnostics;

namespace Sim.Domain.Logic
{
    public class RelayState(string posInp, string negInp)
    {
        private ChainState _relayState = ChainValue.Z;
        private bool _updated = false;
        //public string Name { get; set; } = name;
        public ContactState NormalContact { get => IsHigh() ? ContactValue.T : ContactValue.F; }
        public ContactState PolarContact { get; private set; }
        public bool IsUpdated { get => _updated; }
        public int StartDelay { get; set; } = 0;
        public int EndDelay { get; set; } = 0;

        public string PositiveInputExpression { get; } = posInp;

        private Script<ChainState>? _positiveInputScript;
        public string NegativeInputExpression { get; } = negInp;

        private Script<ChainState>? _negativeInputScript;
        private Script<ChainState>? _inputScript;

        public override string ToString() => $"{_relayState.Value}";

        public string ToLogic() => $"{PositiveInputExpression} ^ {NegativeInputExpression}";

        public async Task<RelayState> Calc(InputContactGroupDto contactState)
        {
            var scriptOptions = ScriptOptions.Default
                .AddReferences(typeof(ChainState).Assembly)
                .AddReferences(typeof(ContactState).Assembly)
                .AddReferences(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly)
                .AddImports("System");


            if (_inputScript is null)
            {
                _inputScript = CSharpScript.Create<ChainState>(ToLogic(), scriptOptions, typeof(InputContactGroupDto));
                _inputScript.Compile();
            }

            var relayNewState = (await _inputScript.RunAsync(contactState, new CancellationToken())).ReturnValue;
            _updated = relayNewState.Value != _relayState.Value;
            _relayState = relayNewState;
            PolarContact = IsHigh() ? IsNegative() : PolarContact;

            return this;
        }

        public async Task<RelayState> CalcFromExternal(InputContactGroupDto contactState, ScriptState? script, string executeCode)
        {
            var relayNewState = (await script!.ContinueWithAsync<ChainState>(executeCode)).ReturnValue;
            _updated = relayNewState.Value != _relayState.Value;
            _relayState = relayNewState;
            PolarContact = IsHigh() ? IsNegative() : PolarContact;

            return this;
        }

        public void Compile(InputContactGroupDto contactState)
        {
            var scriptOptions = ScriptOptions.Default
                .AddReferences(typeof(ChainState).Assembly)
                .AddReferences(typeof(ContactState).Assembly)
                .AddReferences(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly)
                .AddImports("System");

            _inputScript = CSharpScript.Create<ChainState>(ToLogic(), scriptOptions, typeof(InputContactGroupDto));
            _inputScript.Compile();
        }

        private bool IsHigh() => _relayState == ChainValue.P || _relayState == ChainValue.N;
        private bool IsNegative() => _relayState == ChainValue.N;

    }

}

