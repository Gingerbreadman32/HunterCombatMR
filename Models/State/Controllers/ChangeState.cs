﻿using HunterCombatMR.Constants;
using HunterCombatMR.Interfaces.State;
using HunterCombatMR.Managers;
using HunterCombatMR.Models.Components;
using System;

namespace HunterCombatMR.Models.State.Controllers
{
    public struct ChangeState
        : IControllerEffect
    {
        public string ControllerType => StateControllerTypes.ChangeState;
        public int RequiredArguments => 1;

        public void Invoke(int entityId, params object[] args)
        {
            ref var component = ref ComponentManager.GetEntityComponent<EntityStateComponent>(entityId);

            component.SetState(Convert.ToInt32(args[0]));
        }
    }
}