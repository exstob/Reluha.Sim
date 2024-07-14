using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sim.Domain
{

    public enum ChainValue 
    {
        Z, // zero
        P, // positive
        U, // unknown (posisive or negative)
        N, // negative
     }

    public class ChainState
    {
        public ChainValue Value { get; set; }

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

        public static explicit operator ChainState(ChainValue value)
        {
            return new ChainState(value);
        }

        public override string ToString() => $"{Value}";

        public static ChainState operator |(ChainState firstInput, ChainState secondInput)  // apply where different poles are connected
        {
            return (firstInput.Value, secondInput.Value) switch
            {
                (ChainValue.P, ChainValue.N) => new ChainState(ChainValue.U),
                (ChainValue.N, ChainValue.P) => new ChainState(ChainValue.U),
                
                (ChainValue.P, ChainValue.P) => new ChainState(ChainValue.P),
                (ChainValue.N, ChainValue.N) => new ChainState(ChainValue.N),
                
                (ChainValue.Z, ChainValue.Z) => new ChainState(ChainValue.Z),
                
                (ChainValue.Z, ChainValue.P) => new ChainState(ChainValue.P),
                (ChainValue.Z, ChainValue.N) => new ChainState(ChainValue.N),
                (ChainValue.Z, ChainValue.U) => new ChainState(ChainValue.U),
                
                (ChainValue.P, ChainValue.Z) => new ChainState(ChainValue.P),
                (ChainValue.N, ChainValue.Z) => new ChainState(ChainValue.N),
                (ChainValue.U, ChainValue.Z) => new ChainState(ChainValue.U),

                (ChainValue.U, ChainValue.P) => new ChainState(ChainValue.U),
                (ChainValue.U, ChainValue.N) => new ChainState(ChainValue.U),

                (ChainValue.P, ChainValue.U) => new ChainState(ChainValue.U),
                (ChainValue.N, ChainValue.U) => new ChainState(ChainValue.U),

                _ => new ChainState(ChainValue.U)
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

        public static ChainState operator &(ChainState firstInput, ContactValue secondInput) // Mix binary and quaternary
        {
            return (firstInput.Value, secondInput) switch
            {
                (ChainValue.P, ContactValue.F) => new ChainState(ChainValue.Z),
                (ChainValue.N, ContactValue.F) => new ChainState(ChainValue.Z),
                (ChainValue.Z, ContactValue.F) => new ChainState(ChainValue.Z),
                (ChainValue.U, ContactValue.F) => new ChainState(ChainValue.Z),
                (ChainValue.P, ContactValue.T) => new ChainState(ChainValue.P),
                (ChainValue.N, ContactValue.T) => new ChainState(ChainValue.N),
                (ChainValue.Z, ContactValue.T) => new ChainState(ChainValue.Z),
                (ChainValue.U, ContactValue.T) => new ChainState(ChainValue.U),
                _ => new ChainState(ChainValue.U) // Default case
            };
        }

        public static ChainState operator &(ContactValue firstInput, ChainState secondInput) // Mix binary and quaternary
        {
            return (firstInput, secondInput.Value) switch
            {
                (ContactValue.F, ChainValue.P) => new ChainState(ChainValue.Z),
                (ContactValue.F, ChainValue.N) => new ChainState(ChainValue.Z),
                (ContactValue.F, ChainValue.Z) => new ChainState(ChainValue.Z),
                (ContactValue.F, ChainValue.U) => new ChainState(ChainValue.Z),
                (ContactValue.T, ChainValue.P) => new ChainState(ChainValue.P),
                (ContactValue.T, ChainValue.N) => new ChainState(ChainValue.N),
                (ContactValue.T, ChainValue.Z) => new ChainState(ChainValue.Z),
                (ContactValue.T, ChainValue.U) => new ChainState(ChainValue.U),
                _ => new ChainState(ChainValue.U) // Default case
            };
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
            return (state.Value) switch
            {
                (ChainValue.P) => new ChainState(ChainValue.N),
                (ChainValue.N) => new ChainState(ChainValue.P),
                (ChainValue.Z) => new ChainState(ChainValue.U), // under question, perhaps it might be Z
                (ChainValue.U) => new ChainState(ChainValue.Z), // under question, perhaps it might be Z
                _ => new ChainState(ChainValue.Z)
            };
        }
    }
}
