using HunterCombatMR.Components;
using HunterCombatMR.Constants;
using HunterCombatMR.Interfaces.State;
using HunterCombatMR.Managers;
using HunterCombatMR.Messages.EntityStateSystem;

namespace HunterCombatMR.Controllers
{
    /// <summary>
    /// Change the state of the current entity.
    /// args[0] = State number to change to.
    /// </summary>
    public class ChangeState
        : IStateControllerType
    {
        public string Name => StateControllerTypes.ChangeState;
        public int RequiredArguments => 1;

        public void Invoke(int entityId, params string[] args)
        {
            if (!int.TryParse(args[0], out int stateNumber))
                return; // Log

            ref var component = ref ComponentManager.GetEntityComponent<EntityStateComponent>(entityId);
            SystemManager.SendMessage(new ChangeStateMessage(entityId, stateNumber));
        }
    }
}