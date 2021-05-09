using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HunterCombatMR.Events
{
    public sealed class SetPlayerVelocityDirect
        : Event<HunterCombatPlayer>
    {
        private const string _X = "VelocityX";
        private const string _Y = "VelocityY";
        private const string _direc = "ConsiderDirection";

        public SetPlayerVelocityDirect(float xVelocity = 0,
            float yVelocity = 0,
            bool considerDirection = false,
            int length = 1)
            : base((FrameLength)length)
        {
            ModifyParameter(_X, xVelocity);
            ModifyParameter(_Y, yVelocity);
            ModifyParameter(_direc, (considerDirection) ? 1f : 0f);
        }

        public override IEnumerable<EventParameter> DefaultParameters
        {
            get => new List<EventParameter>() { new EventParameter(_X, 0f), new EventParameter(_Y, 0f), new EventParameter(_direc, 0f) };
        }

        public override void InvokeLogic(HunterCombatPlayer entity, Animator animator)
        {
            float x = GetParameterValue(_X);
            if (GetParameterValue(_direc).Equals(1f) && entity.player.direction.Equals(-1))
                x *= -1;

            var velocity = new Vector2(x, GetParameterValue(_Y));
            var moveRequest = new MovementRequest("Direct Velocity Set", velocity, true);
            entity.StateController.MovementInformation.CreateMovementRequest(moveRequest);
        }
    }
}