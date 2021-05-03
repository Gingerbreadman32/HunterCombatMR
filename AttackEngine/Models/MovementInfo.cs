using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public class MovementInfo
    {
        private Queue<KeyValuePair<string, Vector2>> _moveRequests;

        public MovementInfo()
        {
            _moveRequests = new Queue<KeyValuePair<string, Vector2>>();
        }

        public int FinalDirection { get; private set; }

        public Vector2 FinalVelocity { get; private set; }

        public int VanillaDirection { get; private set; }

        public Vector2 VanillaVelocity { get; private set; }

        public int CalculateDirection(int vanillaDirection,
            int actionDirection)
        {
            VanillaDirection = vanillaDirection;
            FinalDirection = actionDirection;

            return FinalDirection;
        }

        public Vector2 CalculateVelocity(Vector2 vanillaVelocity)
        {
            Vector2 baseVelocity = Vector2.Zero;
            VanillaVelocity = vanillaVelocity;

            FinalVelocity = baseVelocity;

            return FinalVelocity;
        }
    }
}