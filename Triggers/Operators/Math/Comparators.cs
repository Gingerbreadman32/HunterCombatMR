namespace HunterCombatMR.Triggers.Operators.Math
{
    internal class Maximum
        : BinaryOperator
    {
        public Maximum()
        {
            Name = nameof(Maximum);
        }

        protected override float Logic(int entityId,
            float left,
            float right)
            => System.Math.Max(left, right);
    }

    internal class Minimum
        : BinaryOperator
    {
        public Minimum()
        {
            Name = nameof(Minimum);
        }

        protected override float Logic(int entityId,
            float left,
            float right)
            => System.Math.Min(left, right);
    }
}