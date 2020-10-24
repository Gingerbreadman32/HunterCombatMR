using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using HunterCombatMR.AttackEngine.Constants;

namespace HunterCombatMR.AttackEngine.Animations
{
    public sealed class StandingAttacks
        : ActionContainer
    {
        public override void Load()
        {
            // - Sword and Shield Attacks

            // -- Standing
            AddSnSStandingAttack2();
        }

        private void AddSnSStandingAttack2()
        {
            int keyframes = 2;
            int holdframes = 2;
            var layers = new List<AnimationLayer>();

            // ---- Head
            var headinfo = new AnimationLayer(LayerNames.Head, new Rectangle(0, 0, 22, 18), 2);
            headinfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(6,4))},
                {1, new LayerFrameInfo(0, new Vector2(8,6))}
            };

            // ---- Front Arm
            var farminfo = new AnimationLayer(LayerNames.FrontArm, new Rectangle(0, 0, 18, 14), 1);
            farminfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(-12,10))},
            };

            // ---- Back Arm
            var barminfo = new AnimationLayer(LayerNames.BackArm, new Rectangle(0, 0, 32, 26), 5);
            barminfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(0,8))},
            };

            // ---- Body
            var bodyinfo = new AnimationLayer(LayerNames.Body, new Rectangle(0, 0, 26, 18), 4);
            bodyinfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(2,14))},
            };

            // ---- Front Leg
            var fleginfo = new AnimationLayer(LayerNames.FrontLeg, new Rectangle(0, 0, 22, 14), 2);
            fleginfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(-4,30))},
            };

            // ---- Back Leg
            var bleginfo = new AnimationLayer(LayerNames.BackLeg, new Rectangle(0, 0, 18, 16), 6);
            bleginfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(10,28))},
            };

            layers.Add(headinfo);
            layers.Add(farminfo);
            layers.Add(barminfo);
            layers.Add(bodyinfo);
            layers.Add(fleginfo);
            layers.Add(bleginfo);

            AnimatedActions.Add("SNS-StandingLL",
                new LayeredAnimatedActionData(
                    new KeyFrameProfile(keyframes, holdframes),
                    layers));
        }
    }
}