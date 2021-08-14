using HunterCombatMR.Interfaces.Entity;
using HunterCombatMR.Interfaces.State;
using HunterCombatMR.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Managers
{
    public sealed class StateControllerManager
        : ManagerBase
    {
        private static List<IControllerEffect> _stateControllers;

        public static bool ControllerExists(string controllerType)
            => _stateControllers.Any(x => x.ControllerType.Equals(controllerType));

        public static void InvokeController(string controllerType,
                    int entityId,
            params object[] args)
        {
            var controller = _stateControllers.Single(x => x.ControllerType.Equals(controllerType));

            if (args.Length < controller.RequiredArguments)
                throw new ArgumentOutOfRangeException($"Controller {controllerType} requires at least {controller.RequiredArguments} arguments!");

            controller.Invoke(entityId, args);
        }

        protected override void OnDispose()
        {
            _stateControllers = null;
        }

        protected override void OnInitialize()
        {
            _stateControllers = new List<IControllerEffect>();
            foreach (var controller in GetType().Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(IControllerEffect)) && !x.IsAbstract))
            {
                _stateControllers.Add(controller as IControllerEffect);
            }
        }
    }
}