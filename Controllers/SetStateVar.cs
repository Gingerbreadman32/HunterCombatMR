using HunterCombatMR.Components;
using HunterCombatMR.Interfaces.State;
using HunterCombatMR.Managers;

namespace HunterCombatMR.Controllers
{
    /// <summary>
    /// Sets one of the current entity's StateVar to a specified value.
    /// args[0] = StateVar Index, args[1] = Value to set
    /// </summary>
    public class SetStateVar
        : IStateControllerType
    {
        public string Name => nameof(SetStateVar);
        public int RequiredArguments => 2;

        public void Invoke(int entityId, params string[] args)
        {
            if (!int.TryParse(args[0], out int varIndex) || !float.TryParse(args[1], out float setValue))
                return; //Log

            ref var component = ref ComponentManager.GetEntityComponent<EntityStateComponent>(entityId);
            component.StateVars[varIndex] = setValue;
        }
    }
}