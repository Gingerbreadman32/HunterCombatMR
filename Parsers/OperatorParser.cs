using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;

namespace HunterCombatMR.Parsers
{
    public abstract class OperatorParser
        : IScriptParser
    {
        public OperatorAssociativity Associativity { get; set; }
        public string FunctionName { get; set; }
        public string Operator { get; set; }

        public int Precedence { get; set; }

        public bool Validate(string input,
            int keyIndex)
            => input[keyIndex].Equals(Operator[0]) && OnValidate(input, keyIndex);

        protected abstract bool OnValidate(string input, int keyIndex);
    }
}