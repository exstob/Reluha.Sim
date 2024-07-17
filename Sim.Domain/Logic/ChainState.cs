using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sim.Domain.Logic
{

    public enum ChainValue
    {
        Z, // zero
        P, // positive
        C, // conflict (posisive or negative) 
        N, // negative
    }

    //public enum ChainValue : byte
    //{
    //    Z = 0b_00, // zero
    //    C = 0b_10, // unknown (posisive or negative)
    //    P = 0b_01, // positive
    //    N = 0b_11// negative
    //}

    public class ChainState
    {
        public ChainValue Value { get; set; }

        public static ChainState P() => new(ChainValue.P);
        public static ChainState N() => new(ChainValue.N);
        public static ChainState Z() => new(ChainValue.Z);
        public static ChainState C() => new(ChainValue.C);
        private ChainState()
        {
            Value = ChainValue.Z;
        }
        public ChainState(ChainValue v)
        {
            Value = v;
        }

        public static implicit operator ChainValue(ChainState chainResult)
        {
            return chainResult.Value;
        }

        public static implicit operator ChainState(ChainValue value)
        {
            return new ChainState(value);
        }

        public override string ToString() => $"{Value}";

        public static ChainState operator |(ChainState firstInput, ChainState secondInput)  // apply where different poles are connected
        {
            return (firstInput.Value, secondInput.Value) switch
            {
                (ChainValue.P, ChainValue.N) => new ChainState(ChainValue.C),
                (ChainValue.N, ChainValue.P) => new ChainState(ChainValue.C),

                (ChainValue.P, ChainValue.P) => new ChainState(ChainValue.P),
                (ChainValue.N, ChainValue.N) => new ChainState(ChainValue.N),

                (ChainValue.Z, ChainValue.Z) => new ChainState(ChainValue.Z),

                (ChainValue.Z, ChainValue.P) => new ChainState(ChainValue.P),
                (ChainValue.Z, ChainValue.N) => new ChainState(ChainValue.N),
                (ChainValue.Z, ChainValue.C) => new ChainState(ChainValue.C),

                (ChainValue.P, ChainValue.Z) => new ChainState(ChainValue.P),
                (ChainValue.N, ChainValue.Z) => new ChainState(ChainValue.N),
                (ChainValue.C, ChainValue.Z) => new ChainState(ChainValue.C),

                (ChainValue.C, ChainValue.P) => new ChainState(ChainValue.C),
                (ChainValue.C, ChainValue.N) => new ChainState(ChainValue.C),

                (ChainValue.P, ChainValue.C) => new ChainState(ChainValue.C),
                (ChainValue.N, ChainValue.C) => new ChainState(ChainValue.C),

                _ => new ChainState(ChainValue.C)
            };
        }

        public static ChainState operator &(ChainState firstInput, ChainState secondInput) // Relay AND operation; aplicable (P & x) & !(N & y) (implication)
        {
            return (firstInput.Value, secondInput.Value) switch
            {
                (ChainValue.P, ChainValue.P) => new ChainState(ChainValue.P),
                (ChainValue.N, ChainValue.N) => new ChainState(ChainValue.N),

                _ => new ChainState(ChainValue.Z) // Default case
            };
        }

        public static ChainState operator &(ChainState firstInput, ChainValue secondInput) // Relay AND operation; aplicable (P & x) & !(N & y) (implication)
        {
            return firstInput & (ChainState)secondInput;
        }

        public static ChainState operator &(ChainValue firstInput, ChainState secondInput) // Relay AND operation; aplicable (P & x) & !(N & y) (implication)
        {
            return (ChainState)firstInput & secondInput;
        }

        public static ChainState operator &(ChainState firstInput, ContactValue secondInput) // Mix BINARY and QUATERNARY
        {
            return (firstInput.Value, secondInput) switch
            {
                (ChainValue.P, ContactValue.F) => new ChainState(ChainValue.Z),
                (ChainValue.N, ContactValue.F) => new ChainState(ChainValue.Z),
                (ChainValue.Z, ContactValue.F) => new ChainState(ChainValue.Z),
                (ChainValue.C, ContactValue.F) => new ChainState(ChainValue.Z),
                (ChainValue.P, ContactValue.T) => new ChainState(ChainValue.P),
                (ChainValue.N, ContactValue.T) => new ChainState(ChainValue.N),
                (ChainValue.Z, ContactValue.T) => new ChainState(ChainValue.Z),
                (ChainValue.C, ContactValue.T) => new ChainState(ChainValue.C),
                _ => new ChainState(ChainValue.C) // Default case
            };
        }

        public static ChainState operator &(ContactValue firstInput, ChainState secondInput) // Mix BINARY and QUATERNARY
        {
            return secondInput & firstInput;
        }

        public static ChainState operator ^(ChainState firstInput, ChainState secondInput) // Relay XOR operation
        {
            return (firstInput.Value, secondInput.Value) switch
            {
                (ChainValue.P, ChainValue.N) => new ChainState(ChainValue.P),
                (ChainValue.N, ChainValue.P) => new ChainState(ChainValue.N),
                _ => new ChainState(ChainValue.Z)
            };
        }

        public static ChainState operator !(ChainState state) // Relay XOR operation
        {
            return state.Value switch
            {
                ChainValue.P => new ChainState(ChainValue.N),
                ChainValue.N => new ChainState(ChainValue.P),
                ChainValue.Z => new ChainState(ChainValue.C), // under question, perhaps it might be Z
                ChainValue.C => new ChainState(ChainValue.Z), // under question, perhaps it might be Z
                _ => new ChainState(ChainValue.Z)
            };
        }
    }
}
