using HunterCombatMR.Enumerations;
using System;
using System.Linq;

namespace HunterCombatMR.Parsers
{
    internal class ComparativeOperatorParser
        : BinaryOperatorParser
    {
        internal ComparativeOperatorParser(string key,
            string function)
            : base(key, function, key.Length, OperatorAssociativity.NonAssociative)
        { }

        protected override bool OnValidate(string input,
            int keyIndex)
        {
            if (input.Count(x => x.Equals('=')) > 1)
                throw new FormatException("Cannot have multiple comparative operators!");

            return base.OnValidate(input, keyIndex);
        }
    }
}