using HunterCombatMR.Enumerations;
using System;

namespace HunterCombatMR.Parsers
{
    internal class RightEncapsulatingOperatorParser
        : OperatorParser
    {
        internal RightEncapsulatingOperatorParser()
        {
            Associativity = OperatorAssociativity.RightParenthesis;
            FunctionName = "";
            Operator = ")";
            Precedence = 5;
        }

        protected override bool OnValidate(string input,
            int keyIndex)
        {
            if (!input.Substring(0, keyIndex).Contains("("))
                throw new FormatException($"Right parenthesis in script at index {keyIndex} does not have a matching left parenthesis!");

            return true;
        }
    }
}