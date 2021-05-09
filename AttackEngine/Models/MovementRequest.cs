using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class MovementRequest
        : INamed
    {
        public MovementRequest(string name,
            Vector2 velocity,
            bool set = false)
        {
            Name = name;
            Velocity = velocity;
            SetVelocity = set;
        }

        public string Name { get; }

        public bool SetVelocity { get; }
        public Vector2 Velocity { get; }
    }
}