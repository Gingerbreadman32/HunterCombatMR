using HunterCombatMR.Interfaces;

namespace HunterCombatMR.Triggers.Operators
{
    internal class Equals
        : IComparatorFunction
    {
        public string Name => nameof(Equals);

        public int RequiredParameters => 2;

        public bool Invoke(int entityId, params string[] args)
        {
            if (float.TryParse(args[0], out float left) && float.TryParse(args[1], out float right))
                return left.Equals(right);

            return args[0].Equals(args[1]);
        }
    }

    internal class NotEquals
        : IComparatorFunction
    {
        public string Name => nameof(NotEquals);

        public int RequiredParameters => 2;

        public bool Invoke(int entityId, params string[] args)
        {
            if (float.TryParse(args[0], out float left) && float.TryParse(args[1], out float right))
                return left.Equals(right);

            return !args[0].Equals(args[1]);
        }
    }
}