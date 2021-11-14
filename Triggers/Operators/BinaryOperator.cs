using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.State;
using HunterCombatMR.Triggers.Operators.Math;
using System;

namespace HunterCombatMR.Triggers.Operators
{
    public abstract class BinaryOperator
        : IStateTriggerFunction
    {
        public string Name { get; protected set; }

        public int RequiredParameters => 2;

        public Type ParameterType => typeof(float);

        public string Value => throw new NotImplementedException();

        public float Invoke(int entityId, params ITriggerFunctionParameter[] args)
        {
            if (!float.TryParse(args[0].Value, out float right) || !float.TryParse(args[1].Value, out float left))
                throw new ArgumentOutOfRangeException(nameof(Add), "One or more values passed in are non-numeric!");

            return Logic(entityId, left, right);
        }

        public string Solve()
        {
            throw new NotImplementedException();
        }

        protected abstract float Logic(int entityId,
            float left,
            float right);
    }
}