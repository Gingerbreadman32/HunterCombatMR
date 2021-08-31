using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HunterCombatMR.Models
{
    public class MovementInfo
    {
        private Queue<MovementRequest> _moveRequests;

        public MovementInfo()
        {
            _moveRequests = new Queue<MovementRequest>();
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
            Vector2 baseVelocity = FinalVelocity;
            VanillaVelocity = vanillaVelocity;

            while (_moveRequests.Count > 0)
            {
                var request = _moveRequests.Dequeue();

                if (request.SetVelocity)
                {
                    baseVelocity = request.Velocity;
                    continue;
                }

                baseVelocity += request.Velocity;
            }

            FinalVelocity = baseVelocity;

            return FinalVelocity;
        }

        public void CreateMovementRequest(MovementRequest request)
        {
            _moveRequests.Enqueue(request);
        }

        public MovementRequest SendLastMovementRequest()
            => _moveRequests.Peek();
    }
}