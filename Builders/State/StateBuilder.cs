using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces.State.Builders;
using HunterCombatMR.Models.State;
using HunterCombatMR.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Builders.State
{
    public static class StateBuilderChainMethods
    {
        public static StateBuilder WithNewController(this StateBuilder builder,
            StateControllerType type,
            object parameter,
            StateTrigger[][] triggers,
            int persistency = 1,
            bool noHitPause = false)
        {
            builder.AddController(new StateController(type, new object[1] { parameter }, triggers, persistency, noHitPause));
            return builder;
        }

        public static StateBuilder WithNewController(this StateBuilder builder,
            StateControllerType type,
            object parameter,
            StateTrigger trigger,
            int persistency = 1,
            bool noHitPause = false,
            int triggerDepth = 1)
        {
            StateTrigger[][] triggers = new StateTrigger[0][];
            ArrayUtils.ResizeAndFillArray(ref triggers, triggerDepth + 1, new StateTrigger[0]);
            ArrayUtils.ResizeAndFillArray(ref triggers[triggerDepth], 1, trigger);
            builder.AddController(new StateController(type, new object[1] { parameter }, triggers, persistency, noHitPause));
            return builder;
        }

        public static StateBuilder WithParameters(this StateBuilder builder,
            int? animation = null,
            bool? hasControl = null,
            Vector2? setVelocity = null,
            bool? ignorePhysics = null)
        {
            builder.SetAnimation = animation;
            builder.HasControl = hasControl;
            builder.SetVelocity = setVelocity;
            builder.IgnorePhysics = ignorePhysics;
            return builder;
        }

        public static StateBuilder WithController(this StateBuilder builder,
                    StateController controller)
        {
            builder.AddController(controller);
            return builder;
        }

        public static StateBuilder WithControllers(this StateBuilder builder,
            IEnumerable<StateController> controllers)
        {
            builder.AddControllers(controllers);
            return builder;
        }

        public static StateBuilder WithEntityStatuses(this StateBuilder builder,
                    EntityWorldStatus worldStatus,
            EntityActionStatus actionStatus)
        {
            builder.ActionStatus = actionStatus;
            builder.WorldStatus = worldStatus;
            return builder;
        }
    }

    public class StateBuilder
        : IStateBuilder
    {
        private EntityActionStatus _action;
        private int? _animation;
        private bool? _control;
        private bool? _nphysics;
        private List<StateController> _stateControllers;
        private Vector2? _velocity;
        private EntityWorldStatus _world;

        public StateBuilder()
        {
            _stateControllers = new List<StateController>();
        }

        public EntityActionStatus ActionStatus { get => _action; set => _action = value; }
        public bool? HasControl { get => _control; set => _control = value; }
        public bool? IgnorePhysics { get => _nphysics; set => _nphysics = value; }
        public int? SetAnimation { get => _animation; set => _animation = value; }
        public Vector2? SetVelocity { get => _velocity; set => _velocity = value; }
        public EntityWorldStatus WorldStatus { get => _world; set => _world = value; }

        public void AddController(StateController controller)
        {
            _stateControllers.Add(controller);
        }

        public void AddControllers(IEnumerable<StateController> controller)
        {
            _stateControllers.AddRange(controller);
        }

        public EntityState Build()
        {
            if (!_stateControllers.Any())
                throw new Exception("State must have controllers! Add controllers using WithStateController!");

            return new EntityState(_stateControllers.ToArray(), new StateDef(_world, _action, _nphysics, _animation, _velocity, _control));
        }
    }
}