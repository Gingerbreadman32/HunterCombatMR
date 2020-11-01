using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using HunterCombatMR.AttackEngine.Constants;
using HunterCombatMR.Enumerations;
using Microsoft.Xna.Framework.Graphics;

namespace HunterCombatMR.AttackEngine.Animations
{
    public sealed class PlayerStandingAttacks
        : ActionContainer
    {
        private const AnimationType _animtype = AnimationType.Player;

        public override void Load()
        {
            // - Sword and Shield Attacks

            // -- Standing
            AddSnSStandingAttack2();
        }

        private void AddSnSStandingAttack2()
        {
            int keyframes = 8;
            int holdframes = 12;
            var layers = new List<AnimationLayer>();

            // ---- Head
            var headinfo = new AnimationLayer(LayerNames.Head, new Rectangle(0, 0, 22, 18), DefaultPlayerLayerDepths.Head);
            headinfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(2,4))},
                {1, new LayerFrameInfo(0, new Vector2(4,6))},
                {2, new LayerFrameInfo(1, new Vector2(2,4), DefaultPlayerLayerDepths.Body)},
                {3, new LayerFrameInfo(1, new Vector2(2,2))},
                {4, new LayerFrameInfo(2, new Vector2(2,4))},
                {6, new LayerFrameInfo(3, new Vector2(2,4))},
                {7, new LayerFrameInfo(0, new Vector2(2,4))}
            };
            headinfo.Frames.Add(5, new LayerFrameInfo(headinfo.Frames[2], headinfo.Frames[2].LayerDepthOverride.GetValueOrDefault(DefaultPlayerLayerDepths.Body)));
            headinfo.Initialize();

            // ---- Front Arm
            var farminfo = new AnimationLayer(LayerNames.FrontArm, new Rectangle(0, 0, 18, 14), DefaultPlayerLayerDepths.FrontArm);
            farminfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(-12,10))},
                {1, new LayerFrameInfo(1, new Vector2(-4,20))},
                {2, new LayerFrameInfo(0, new Vector2(16,12), flip: SpriteEffects.FlipHorizontally)},
                {3, new LayerFrameInfo(2, new Vector2(18,4), DefaultPlayerLayerDepths.Body)},
                {4, new LayerFrameInfo(2, new Vector2(18,6), DefaultPlayerLayerDepths.Body)},
                {6, new LayerFrameInfo(3, new Vector2(-16,16))},
                {7, new LayerFrameInfo(0, new Vector2(-12,16), flip:SpriteEffects.FlipVertically)}
            };
            farminfo.Frames.Add(5, new LayerFrameInfo(farminfo.Frames[2], farminfo.DefaultDepth));
            farminfo.Initialize();

            // ---- Back Arm
            var barminfo = new AnimationLayer(LayerNames.BackArm, new Rectangle(0, 0, 32, 26), DefaultPlayerLayerDepths.BackArm);
            barminfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(0,8))},
                {1, new LayerFrameInfo(1, new Vector2(0,8))},
                {2, new LayerFrameInfo(1, new Vector2(-2,8))},
                {3, new LayerFrameInfo(2, new Vector2(0,8))},
                {4, new LayerFrameInfo(2, new Vector2(2,10))},
                {6, new LayerFrameInfo(3, new Vector2(0,8))},
                {7, new LayerFrameInfo(4, new Vector2(0,8))}
            };
            barminfo.Frames.Add(5, new LayerFrameInfo(barminfo.Frames[2], barminfo.DefaultDepth));
            barminfo.Initialize();

            // ---- Body
            var bodyinfo = new AnimationLayer(LayerNames.Body, new Rectangle(0, 0, 26, 18), DefaultPlayerLayerDepths.Body);
            bodyinfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(2,14))},
                {1, new LayerFrameInfo(1, new Vector2(2,14))},
                {2, new LayerFrameInfo(2, new Vector2(2,14), DefaultPlayerLayerDepths.Head) },
                {3, new LayerFrameInfo(3, new Vector2(2,14), DefaultPlayerLayerDepths.FrontArm) },
                {4, new LayerFrameInfo(4, new Vector2(2,14), DefaultPlayerLayerDepths.FrontArm) },
                {6, new LayerFrameInfo(5, new Vector2(2,14))},
                {7, new LayerFrameInfo(6, new Vector2(2,14))}
            };
            bodyinfo.Frames.Add(5, new LayerFrameInfo(bodyinfo.Frames[2], bodyinfo.Frames[2].LayerDepthOverride.GetValueOrDefault(DefaultPlayerLayerDepths.Head)));
            bodyinfo.Initialize();

            // ---- Front Leg
            var fleginfo = new AnimationLayer(LayerNames.FrontLeg, new Rectangle(0, 0, 22, 14), DefaultPlayerLayerDepths.FrontLeg);
            fleginfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(-4,30))},
                {1, new LayerFrameInfo(1, new Vector2(-4,30))},
                {2, new LayerFrameInfo(2, new Vector2(-4,30))},
                {3, new LayerFrameInfo(3, new Vector2(-4,30))},
                {4, new LayerFrameInfo(3, new Vector2(-2,30))},
                {6, new LayerFrameInfo(4, new Vector2(-4,30))},
                {7, new LayerFrameInfo(5, new Vector2(-4,30))}
            };
            fleginfo.Frames.Add(5, new LayerFrameInfo(fleginfo.Frames[2], fleginfo.DefaultDepth));
            fleginfo.Initialize();

            // ---- Back Leg
            var bleginfo = new AnimationLayer(LayerNames.BackLeg, new Rectangle(0, 0, 18, 16), DefaultPlayerLayerDepths.BackLeg);
            bleginfo.Frames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(10,28))},
                {1, new LayerFrameInfo(0, new Vector2(10,28))},
                {2, new LayerFrameInfo(1, new Vector2(10,28))},
                {3, new LayerFrameInfo(2, new Vector2(10,28))},
                {4, new LayerFrameInfo(1, new Vector2(12,28))},
                {6, new LayerFrameInfo(3, new Vector2(10,28))},
                {7, new LayerFrameInfo(4, new Vector2(10,28))}
            };
            bleginfo.Frames.Add(5, new LayerFrameInfo(bleginfo.Frames[2], bleginfo.DefaultDepth));
            bleginfo.Initialize();

            layers.Add(headinfo);
            layers.Add(farminfo);
            layers.Add(barminfo);
            layers.Add(bodyinfo);
            layers.Add(fleginfo);
            layers.Add(bleginfo);

            var timings = new Dictionary<int, int>() { { 2, 4 }, { 3, 8 }, { 4, 14 }, { 5, 4 }, { 7, 52 } };

            AnimatedActions.Add("SNS-StandingLL",
                new LayeredAnimatedActionData(new KeyFrameProfile(keyframes, holdframes, timings),
                    layers,
                    _animtype));
        }
    }
}