using HunterCombatMR.Interfaces.State;
using HunterCombatMR.Triggers.Operators.Math;
using System;

namespace HunterCombatMR.Triggers.Operators
{
    public abstract class UniaryOperator
        : IStateTriggerFunction
    {
        public string Name { get; protected set; }

        public int RequiredParameters => 1;

        public float Invoke(int entityId, params string[] args)
        {
            if (!float.TryParse(args[0], out float parameter))
                throw new ArgumentOutOfRangeException(nameof(Add), "One or more values passed in are non-numeric!");

            return Logic(entityId, parameter);
        }

        protected abstract float Logic(int entityId,
            float parameter);
    }
}