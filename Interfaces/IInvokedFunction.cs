namespace HunterCombatMR.Interfaces
{
    public interface IInvokedFunction
        : IInternallyNamed,
        IParameterized
    {
        object Invoke(int entityId, params ITriggerFunctionParameter[] args);
    }
}