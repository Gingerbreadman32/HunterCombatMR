using HunterCombatMR.Interfaces;
using HunterCombatMR.Managers;
using System;
using System.Diagnostics;
using System.Linq;

namespace HunterCombatMR.Models.Trigger
{
    [DebuggerDisplay("ToString()")]
    public class TriggerFunctionElement
        : ITriggerFunctionParameter
    {
        public TriggerFunctionElement(string functionName,
            params ITriggerFunctionParameter[] parameters)
        {
            Function = functionName;
            Parameters = parameters;
        }

        public string Function { get; }

        public string ParameterDisplay { get => string.Join(",", Parameters.Select(x => x.ToString()).ToArray()); }
        public ITriggerFunctionParameter[] Parameters { get; }

        public string Value { get => Solve(); }

        public int EntityId { get; set; }

        public string Solve()
        {
            return TriggerScriptManager.GetTriggerFunctionResult<T>(Function, EntityId, Parameters);
        }

        public override string ToString()
        {
            return $"{Function}({ParameterDisplay})";
        }
    }
}