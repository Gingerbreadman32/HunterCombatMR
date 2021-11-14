namespace HunterCombatMR.Interfaces
{
    public interface ITriggerFunctionParameter
        : IScriptElement
    {
        int EntityId { get; set; }

        string Solve();
    }
}