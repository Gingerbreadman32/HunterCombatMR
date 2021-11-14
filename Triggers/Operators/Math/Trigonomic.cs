namespace HunterCombatMR.Triggers.Operators.Math
{
    internal class Cosine
        : UniaryOperator
    {
        public Cosine()
        {
            Name = nameof(Cosine);
        }

        protected override float Logic(int entityId,
            float parameter)
            => (float)System.Math.Cos(parameter);
    }

    internal class Sine
        : UniaryOperator
    {
        public Sine()
        {
            Name = nameof(Sine);
        }

        protected override float Logic(int entityId,
            float parameter)
            => (float)System.Math.Sin(parameter);
    }
}