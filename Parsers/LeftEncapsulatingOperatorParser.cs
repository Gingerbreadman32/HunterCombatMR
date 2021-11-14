using HunterCombatMR.Enumerations;
using System;

namespace HunterCombatMR.Parsers
{
    internal class LeftEncapsulatingOperatorParser
        : OperatorParser
    {
        internal LeftEncapsulatingOperatorParser()
            : base()
        {
            Associativity = OperatorAssociativity.LeftParenthesis;
            FunctionName = "";
            Operator = FunctionName + "(";
            Precedence = 5;
        }

        internal LeftEncapsulatingOperatorParser(string key,
            string function,
            int precedence = 6)
            : base()
        {
            if (string.IsNullOrWhiteSpace(function))
                throw new ArgumentNullException("Function name must be provided!");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("An operator key must be provided!");

            Associativity = OperatorAssociativity.Function;
            FunctionName = function.Trim();
            Operator = key + "(";
            Precedence = precedence;
        }

        protected override bool OnValidate(string input,
            int keyIndex)
        {
            if (!input.Substring(keyIndex).Contains(")"))
                throw new FormatException($"Left parenthesis in script at index {keyIndex} does not have a matching right parenthesis!");

            return true;
        }
    }
}