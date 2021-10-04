using HunterCombatMR.Interfaces.State;
using HunterCombatMR.Services;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Managers
{
    public sealed class StateControllerManager
        : ManagerBase
    {
        private static IEnumerable<IStateControllerType> _controllerActions;

        public static bool ControllerExists(string controllerType)
            => _controllerActions.Any(x => x.Name.Equals(controllerType));

        public static void InvokeController(string controllerType,
            int entityId,
            params string[] args)
        {
            var controller = _controllerActions.Single(x => x.Name.Equals(controllerType));

            if (args.Length < controller.RequiredArguments)
                throw new ArgumentOutOfRangeException($"Controller {controllerType} requires at least {controller.RequiredArguments} arguments!");

            controller.Invoke(entityId, args);
        }

        protected override void OnDispose()
        {
            _controllerActions = null;
        }

        protected override void OnInitialize()
        {
            _controllerActions = ReflectionUtils.InstatiateTypesFromInterface<IStateControllerType>();
        }
    }
}