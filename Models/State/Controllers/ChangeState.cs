using HunterCombatMR.Constants;
using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.State;
using HunterCombatMR.Managers;
using HunterCombatMR.Models.Components;
using System;

namespace HunterCombatMR.Models.State.Controllers
{
    public struct ChangeState
        : IControllerEffect
    {
        public int RequiredArguments => 1;

        public string ControllerType => StateControllerTypes.ChangeState;

        public void Invoke(in IModEntity entity, params object[] args)
        {
            ref var component = ref ComponentManager.GetEntityComponent<EntityStateComponent>(entity);

            component.SetState(Convert.ToInt32(args[0]));
        }
    }
}