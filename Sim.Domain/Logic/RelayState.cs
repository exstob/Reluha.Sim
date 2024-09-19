using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

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

        public override string ToString() => $"{_relayState.Value}";

        public string ToLogic() => $"{PositiveInputExpression} ^ {NegativeInputExpression}";

        public async Task<RelayState> Calc(InputContactGroupDto contactState)
        {
            var scriptOptions = ScriptOptions.Default
                .AddReferences(typeof(ChainState).Assembly)
                .AddReferences(typeof(ContactState).Assembly)
                .AddReferences(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly)
                .AddImports("System");


            if (_positiveInputScript is null)
            {
                _positiveInputScript = CSharpScript.Create<ChainState>(PositiveInputExpression, scriptOptions, typeof(InputContactGroupDto));
                _positiveInputScript.Compile();
            }

            var posResult = (await _positiveInputScript.RunAsync(contactState, new CancellationToken())).ReturnValue;

            if (_negativeInputScript is null)
            {
                _negativeInputScript = CSharpScript.Create<ChainState>(NegativeInputExpression, scriptOptions, typeof(InputContactGroupDto));
                _negativeInputScript.Compile();
            }

            var negResult = (await _negativeInputScript.RunAsync(contactState, new CancellationToken())).ReturnValue;

            var relayNewState = posResult ^ negResult;
            _updated = relayNewState.Value != _relayState.Value;
            _relayState = relayNewState;
            PolarContact = IsHigh() ? IsNegative() : PolarContact;

            return this;
        }

        private bool IsHigh() => _relayState == ChainValue.P || _relayState == ChainValue.N;
        private bool IsNegative() => _relayState == ChainValue.N;

    }

}

