using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Sim.Domain;

namespace Sim.Domain
{
    public class RelayState(string name, string posInp, string negInp)
    {
        private ChainState _relayState = ChainValue.Z;
        public string Name { get; set; } = name;
        public ContactState NormalContact { get; set; } = ContactValue.F;
        public ContactState PolarContact { get; set; } = ContactValue.F;
        public int StartDelay { get; set; } = 0;
        public int EndDelay { get; set; } = 0;

        public string PositiveInputExpression { get; } = posInp;

        private Script<ChainState>? _positiveInputScript;
        public string NegativeInputExpression { get; } = negInp;

        private Script<ChainState>? _negativeInputScript;

        public async Task<ChainState> Calc(GlobalsExpando contactState) 
        {
            var scriptOptions = ScriptOptions.Default
                .AddReferences(typeof(ChainState).Assembly)
                .AddReferences(typeof(ContactState).Assembly)
                .AddReferences(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly)
                .AddImports("System");


            if (_positiveInputScript is null) 
            {
                _positiveInputScript = CSharpScript.Create<ChainState>(PositiveInputExpression, scriptOptions, typeof(GlobalsExpando));
                _positiveInputScript.Compile();
            }

            var posResult = (await _positiveInputScript.RunAsync(contactState, new CancellationToken())).ReturnValue;

            //var posResult = await CSharpScript.EvaluateAsync<ChainState>(
            //    PositiveInputExpression,
            //    scriptOptions,
            //    globals);

            if (_negativeInputScript is null)
            {
                _negativeInputScript = CSharpScript.Create<ChainState>(NegativeInputExpression, scriptOptions, typeof(GlobalsExpando));
                _negativeInputScript.Compile();
            }

            var negResult = (await _negativeInputScript.RunAsync(contactState, new CancellationToken())).ReturnValue;

            //var negResult = await CSharpScript.EvaluateAsync<ChainState>(
            //    PositiveInputExpression,
            //    scriptOptions,
            //    globals);

            ///_relayState = (ChainValue.P & posResult) ^ negResult;  /// if the relay with Diod (one way current)
            _relayState = posResult ^ negResult;

            NormalContact = IsHigh() ? ContactValue.T : ContactValue.F;
            PolarContact = IsNegative() ? ContactValue.T : ContactValue.F;
            return _relayState;
        }

        private bool IsHigh() => _relayState == ChainValue.P || _relayState == ChainValue.N;
        private bool IsNegative() => _relayState == ChainValue.N;

    }



}
//public class GlobalsExpando
//{
//    public ChainState PP { get; set; }
//    public ContactState A { get; set; }
//    public ContactState B { get; set; }
//    public ChainState NN { get; set; }
//    public ContactState C { get; set; }
//    public ContactState D { get; set; }
//}

public class GlobalsExpando
{
    public dynamic x { get; set; }
}


