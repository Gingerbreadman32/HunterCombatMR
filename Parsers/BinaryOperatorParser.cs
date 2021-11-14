using HunterCombatMR.Enumerations;
using System;

namespace HunterCombatMR.Parsers
{
    internal class BinaryOperatorParser
        : OperatorParser
    {
        internal BinaryOperatorParser(string key,
            string function,
            int precedence,
            OperatorAssociativity associativity = OperatorAssociativity.Left)
        {
            FunctionName = function;
            Operator = key;
            Precedence = precedence;
            Associativity = associativity;
        }

        protected override bool OnValidate(string input,
            int keyIndex)
        {
            var splitBack = input.Substring(0, keyIndex);
            var splitFront = input.Substring(keyIndex);

            if (string.IsNullOrWhiteSpace(splitBack)
                || string.IsNullOrWhiteSpace(splitFront))
                throw new FormatException($"Must be values on both sides of comparative operator at index {keyIndex}!");

            return true;
        }
    }
}