namespace HunterCombatMR.Interfaces
{
    public interface ITypedInvokedFunction<out T>
        : IInvokedFunction
    {
        T Invoke(int entityId, params ITriggerFunctionParameter[] args);
    }
}