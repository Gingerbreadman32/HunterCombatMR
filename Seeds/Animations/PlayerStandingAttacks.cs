using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Constants;
using HunterCombatMR.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace HunterCombatMR.Seeds.Animations
{
    internal sealed class PlayerStandingAttacks
        : AnimationSeed
    {
        private const string _texturePath = "HunterCombatMR/Textures/SnS/Limbs/";
        private const string _textureSuffix = "Frames";

        internal override void Load()
        {
            // - Sword and Shield Attacks

            // -- Standing
            AddSnSStandingAttack2();
        }

        // Lol this is already outdated
        private void AddSnSStandingAttack2()
        {
            int keyframes = 8;
            int holdframes = 12;
            var layers = new List<AnimationLayer>();

            // ---- Head
            var headinfo = new AnimationLayer(LayerNames.Head, new Rectangle(0, 0, 22, 18), CreateTextureString(LayerNames.Head), DefaultPlayerLayerDepths.Head);
            headinfo.KeyFrames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(2,4))},
                {1, new LayerFrameInfo(0, new Vector2(4,6))},
                {2, new LayerFrameInfo(1, new Vector2(2,4), DefaultPlayerLayerDepths.Body)},
                {3, new LayerFrameInfo(1, new Vector2(2,2))},
                {4, new LayerFrameInfo(2, new Vector2(2,4))},
                {6, new LayerFrameInfo(3, new Vector2(2,4))},
                {7, new LayerFrameInfo(0, new Vector2(2,4))}
            };
            headinfo.KeyFrames.Add(5, new LayerFrameInfo(headinfo.KeyFrames[2], headinfo.KeyFrames[2].LayerDepthOverride.GetValueOrDefault(DefaultPlayerLayerDepths.Body)));
            headinfo.Initialize();

            // ---- Front Arm
            var farminfo = new AnimationLayer(LayerNames.FrontArm, new Rectangle(0, 0, 18, 14), CreateTextureString(LayerNames.FrontArm), DefaultPlayerLayerDepths.FrontArm);
            farminfo.KeyFrames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(-12,10))},
                {1, new LayerFrameInfo(1, new Vector2(-4,20))},
                {2, new LayerFrameInfo(0, new Vector2(16,12), flip: SpriteEffects.FlipHorizontally)},
                {3, new LayerFrameInfo(2, new Vector2(18,4), DefaultPlayerLayerDepths.Body)},
                {4, new LayerFrameInfo(2, new Vector2(18,6), DefaultPlayerLayerDepths.Body)},
                {6, new LayerFrameInfo(3, new Vector2(-16,16))},
                {7, new LayerFrameInfo(0, new Vector2(-12,16), flip:SpriteEffects.FlipVertically)}
            };
            farminfo.KeyFrames.Add(5, new LayerFrameInfo(farminfo.KeyFrames[2], farminfo.DefaultDepth));
            farminfo.Initialize();

            // ---- Back Arm
            var barminfo = new AnimationLayer(LayerNames.BackArm, new Rectangle(0, 0, 32, 26), CreateTextureString(LayerNames.BackArm), DefaultPlayerLayerDepths.BackArm);
            barminfo.KeyFrames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(0,8))},
                {1, new LayerFrameInfo(1, new Vector2(0,8))},
                {2, new LayerFrameInfo(1, new Vector2(-2,8))},
                {3, new LayerFrameInfo(2, new Vector2(0,8))},
                {4, new LayerFrameInfo(2, new Vector2(2,10))},
                {6, new LayerFrameInfo(3, new Vector2(0,8))},
                {7, new LayerFrameInfo(4, new Vector2(0,8))}
            };
            barminfo.KeyFrames.Add(5, new LayerFrameInfo(barminfo.KeyFrames[2], barminfo.DefaultDepth));
            barminfo.Initialize();

            // ---- Body
            var bodyinfo = new AnimationLayer(LayerNames.Body, new Rectangle(0, 0, 26, 18), CreateTextureString(LayerNames.Body), DefaultPlayerLayerDepths.Body);
            bodyinfo.KeyFrames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(2,14))},
                {1, new LayerFrameInfo(1, new Vector2(2,14))},
                {2, new LayerFrameInfo(2, new Vector2(2,14), DefaultPlayerLayerDepths.Head) },
                {3, new LayerFrameInfo(3, new Vector2(2,14), DefaultPlayerLayerDepths.FrontArm) },
                {4, new LayerFrameInfo(4, new Vector2(2,14), DefaultPlayerLayerDepths.FrontArm) },
                {6, new LayerFrameInfo(5, new Vector2(2,14))},
                {7, new LayerFrameInfo(6, new Vector2(2,14))}
            };
            bodyinfo.KeyFrames.Add(5, new LayerFrameInfo(bodyinfo.KeyFrames[2], bodyinfo.KeyFrames[2].LayerDepthOverride.GetValueOrDefault(DefaultPlayerLayerDepths.Head)));
            bodyinfo.Initialize();

            // ---- Front Leg
            var fleginfo = new AnimationLayer(LayerNames.FrontLeg, new Rectangle(0, 0, 22, 14), CreateTextureString(LayerNames.FrontLeg), DefaultPlayerLayerDepths.FrontLeg);
            fleginfo.KeyFrames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(-4,30))},
                {1, new LayerFrameInfo(1, new Vector2(-4,30))},
                {2, new LayerFrameInfo(2, new Vector2(-4,30))},
                {3, new LayerFrameInfo(3, new Vector2(-4,30))},
                {4, new LayerFrameInfo(3, new Vector2(-2,30))},
                {6, new LayerFrameInfo(4, new Vector2(-4,30))},
                {7, new LayerFrameInfo(5, new Vector2(-4,30))}
            };
            fleginfo.KeyFrames.Add(5, new LayerFrameInfo(fleginfo.KeyFrames[2], fleginfo.DefaultDepth));
            fleginfo.Initialize();

            // ---- Back Leg
            var bleginfo = new AnimationLayer(LayerNames.BackLeg, new Rectangle(0, 0, 18, 16), CreateTextureString(LayerNames.BackLeg), DefaultPlayerLayerDepths.BackLeg);
            bleginfo.KeyFrames = new Dictionary<int, LayerFrameInfo>()
            {
                {0, new LayerFrameInfo(0, new Vector2(10,28))},
                {1, new LayerFrameInfo(0, new Vector2(10,28))},
                {2, new LayerFrameInfo(1, new Vector2(10,28))},
                {3, new LayerFrameInfo(2, new Vector2(10,28))},
                {4, new LayerFrameInfo(1, new Vector2(12,28))},
                {6, new LayerFrameInfo(3, new Vector2(10,28))},
                {7, new LayerFrameInfo(4, new Vector2(10,28))}
            };
            bleginfo.KeyFrames.Add(5, new LayerFrameInfo(bleginfo.KeyFrames[2], bleginfo.DefaultDepth));
            bleginfo.Initialize();

            layers.Add(headinfo);
            layers.Add(farminfo);
            layers.Add(barminfo);
            layers.Add(bodyinfo);
            layers.Add(fleginfo);
            layers.Add(bleginfo);

            var timings = new SortedList<int, FrameLength>() { { 2, 4.ToFLength() }, { 3, 8.ToFLength() }, { 4, 14.ToFLength() }, { 5, 4.ToFLength() }, { 7, 52.ToFLength() } };

            AnimatedActions.Add("SNS-StandingLL",
                new LayerData(new KeyFrameProfile(keyframes.ToFLength(), holdframes.ToFLength(), timings),
                    layers));
        }

        private string CreateTextureString(string layerName)
                            => $"{_texturePath}{layerName.Split('_')[1]}{_textureSuffix}";
    }
}