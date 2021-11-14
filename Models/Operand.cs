using HunterCombatMR.Enumerations;
using System.Diagnostics;

namespace HunterCombatMR.Models
{
    [DebuggerDisplay("{Value} - {Category}")]
    public struct Operand
    {
        public Operand(string value,
            OperandCategory category)
        {
            Category = category;
            Value = value;
        }

        public OperandCategory Category { get; }
        public string Value { get; }

        public static Operand FunctionOperand(string functionName)
            => new Operand(functionName, OperandCategory.Function);
    }
}