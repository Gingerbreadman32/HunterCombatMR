using HunterCombatMR.Interfaces;
using System;

namespace HunterCombatMR.Models
{
    public struct FunctionName
        : IScriptElement<string>,
        IEquatable<string>
    {
        public FunctionName(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static implicit operator FunctionName(string b)
                    => new FunctionName(b);

        public bool Equals(string other)
            => Value.Equals(other);

        public string Solve(int entityId = -1)
                    => Value;

        public override string ToString()
            => Solve();
    }
}