using HunterCombatMR.Enumerations;

namespace HunterCombatMR.Interfaces
{
    internal interface IScriptParser
    {
        string Operator { get; }

        string FunctionName { get; }

        int Precedence { get; }

        OperatorAssociativity Associativity { get; }

        bool Validate(string input, int keyIndex);
    }
}