using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sim.Domain
{

    public enum ChainValue 
    {
        Z, // zero
        P, // positive
        U, // unknown (posisive or negative)
        N, // negative
     }

    public enum ContactValue
    {
        F, // false
        T, // trye
    }

    public class ChainResult
    {
        public ChainValue Value { get; set; }

        private ChainResult()
        {
            Value = ChainValue.Z;
        }
        public ChainResult(ChainValue v) 
        { 
            Value = v;
        }

        public static implicit operator ChainValue(ChainResult chainResult)
        {
            return chainResult.Value;
        }

        public override string ToString() => $"{Value}";

        public static ChainValue operator |(ChainResult lhs, ChainResult rhs)  // apply where different poles are connected
        {
            return (lhs.Value, rhs.Value) switch
            {
                (ChainValue.P, ChainValue.N) => new ChainResult(ChainValue.U),
                (ChainValue.N, ChainValue.P) => new ChainResult(ChainValue.U),
                
                (ChainValue.P, ChainValue.P) => new ChainResult(ChainValue.P),
                (ChainValue.N, ChainValue.N) => new ChainResult(ChainValue.N),
                
                (ChainValue.Z, ChainValue.Z) => new ChainResult(ChainValue.Z),
                
                (ChainValue.Z, ChainValue.P) => new ChainResult(ChainValue.P),
                (ChainValue.Z, ChainValue.N) => new ChainResult(ChainValue.N),
                (ChainValue.Z, ChainValue.U) => new ChainResult(ChainValue.U),
                
                (ChainValue.P, ChainValue.Z) => new ChainResult(ChainValue.P),
                (ChainValue.N, ChainValue.Z) => new ChainResult(ChainValue.N),
                (ChainValue.U, ChainValue.Z) => new ChainResult(ChainValue.U),

                (ChainValue.U, ChainValue.P) => new ChainResult(ChainValue.U),
                (ChainValue.U, ChainValue.N) => new ChainResult(ChainValue.U),

                (ChainValue.P, ChainValue.U) => new ChainResult(ChainValue.U),
                (ChainValue.N, ChainValue.U) => new ChainResult(ChainValue.U),

                _ => ChainValue.U // Default case
            };
        }

        public static ChainValue operator &(ChainResult lhs, ContactValue rhs)
        {
            return (lhs.Value, rhs) switch
            {
                (ChainValue.P, ContactValue.F) => new ChainResult(ChainValue.Z),
                (ChainValue.N, ContactValue.F) => new ChainResult(ChainValue.Z),
                (ChainValue.Z, ContactValue.F) => new ChainResult(ChainValue.Z),
                (ChainValue.U, ContactValue.F) => new ChainResult(ChainValue.Z),
                (ChainValue.P, ContactValue.T) => new ChainResult(ChainValue.P),
                (ChainValue.N, ContactValue.T) => new ChainResult(ChainValue.N),
                (ChainValue.Z, ContactValue.T) => new ChainResult(ChainValue.Z),
                (ChainValue.U, ContactValue.T) => new ChainResult(ChainValue.U),
                _ => ChainValue.U // Default case
            };
        }

        public static ChainValue operator &(ContactValue lhs, ChainResult rhs)
        {
            return (lhs, rhs.Value) switch
            {
                (ContactValue.F, ChainValue.P) => new ChainResult(ChainValue.Z),
                (ContactValue.F, ChainValue.N) => new ChainResult(ChainValue.Z),
                (ContactValue.F, ChainValue.Z) => new ChainResult(ChainValue.Z),
                (ContactValue.F, ChainValue.U) => new ChainResult(ChainValue.Z),
                (ContactValue.T, ChainValue.P) => new ChainResult(ChainValue.P),
                (ContactValue.T, ChainValue.N) => new ChainResult(ChainValue.N),
                (ContactValue.T, ChainValue.Z) => new ChainResult(ChainValue.Z),
                (ContactValue.T, ChainValue.U) => new ChainResult(ChainValue.U),
                _ => ChainValue.U // Default case
            };
        }
    }
}
