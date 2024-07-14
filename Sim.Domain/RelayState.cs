using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Domain
{
    public class RelayState(string posInp, string negInp)
    {
        private ChainState _relayState = ChainValue.Z;
        public ContactState NormalContact { get; set; } = ContactValue.F;
        public ContactState PolarContact { get; set; } = ContactValue.F;
        public int StartDelay { get; set; } = 0;
        public int EndDelay { get; set; } = 0;

        public string PositiveInputExpression { get; } = posInp;
        public string NegativeInputExpression { get; } = negInp;

        public async Task<ChainState> Calc(ChainState x1, ChainState x2) 
        {
            //_relayState = PositiveInputExpression + NegativeInputExpression;

            var globals = new Globals { A = x1, B = x2 };

            var scriptOptions = ScriptOptions.Default
                .AddReferences(typeof(ChainState).Assembly)
                .AddReferences(typeof(ContactState).Assembly)
                .AddImports("System");


            var posCompile = CSharpScript.Create<ChainState>(PositiveInputExpression,scriptOptions, typeof(Globals));
            posCompile.Compile();


            var posResult = (await posCompile.RunAsync(globals, new CancellationToken())).ReturnValue;

            //var posResult = await CSharpScript.EvaluateAsync<ChainState>(
            //    PositiveInputExpression,
            //    scriptOptions,
            //    globals);

            var negResult = await CSharpScript.EvaluateAsync<ChainState>(
                PositiveInputExpression,
                scriptOptions,
                globals);


            _relayState = posResult ^ negResult;

            NormalContact = IsHigh() ? ContactValue.T : ContactValue.F;
            PolarContact = IsNegative() ? ContactValue.T : ContactValue.F;
            return _relayState;
        }

        private bool IsHigh() => _relayState == ChainValue.P || _relayState == ChainValue.N;
        private bool IsNegative() => _relayState == ChainValue.N;

    }

    public class Globals
    {
        public ChainState A;
        public ChainState B;
    }

}
