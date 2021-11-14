namespace HunterCombatMR.Triggers.Operators.Math
{
    internal class Add
        : BinaryOperator
    {
        public Add()
        {
            Name = nameof(Add);
        }

        protected override float Logic(int entityId,
            float left,
            float right)
            => left + right;
    }

    internal class Divide
            : BinaryOperator
    {
        public Divide()
        {
            Name = nameof(Divide);
        }

        protected override float Logic(int entityId,
            float left,
            float right)
            => left / right;
    }

    internal class Multiply
            : BinaryOperator
    {
        public Multiply()
        {
            Name = nameof(Multiply);
        }

        protected override float Logic(int entityId,
            float left,
            float right)
            => left * right;
    }

    internal class Subtract
            : BinaryOperator
    {
        public Subtract()
        {
            Name = nameof(Subtract);
        }

        protected override float Logic(int entityId,
            float left,
            float right)
            => left - right;
    }
}