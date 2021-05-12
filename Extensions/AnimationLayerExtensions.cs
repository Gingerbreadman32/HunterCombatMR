using HunterCombatMR.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HunterCombatMR.Extensions
{
    public static class AnimationLayerExtensions
    {
        public static byte GetDepth(this AnimationLayer layer,
            int keyFrame)
                    => layer.KeyFrames[keyFrame].LayerDepth;

        public static Rectangle GetFrameRectangle(this AnimationLayer layer,
                    int currentKeyFrame)
                    => layer.SpriteFrameRectangle.SetSheetPositionFromFrame(layer.KeyFrames[currentKeyFrame].SpriteFrame);

        public static SpriteEffects GetOrientation(this AnimationLayer layer,
            int keyFrame)
                => layer.KeyFrames[keyFrame].SpriteOrientation;

        public static Vector2 GetPosition(this AnimationLayer layer,
            int keyFrame)
                    => layer.KeyFrames[keyFrame].Position;

        public static float GetRotation(this AnimationLayer layer,
            int keyFrame)
                => layer.KeyFrames[keyFrame].Rotation;

        public static int GetTextureFrame(this AnimationLayer layer,
            int keyFrame)
                => layer.KeyFrames[keyFrame].SpriteFrame;

        public static int GetTotalTextureFrames(this AnimationLayer layer)
                        => layer.Texture.Height / layer.SpriteFrameRectangle.Height;

        public static void SetDepth(this AnimationLayer layer,
            int keyFrameIndex,
            byte depth)
        {
            byte? newDepth = depth;
            layer.KeyFrames[keyFrameIndex] = new LayerFrameInfo(layer.KeyFrames[keyFrameIndex], newDepth.Value) { LayerDepthOverride = newDepth == layer.DefaultDepth ? null : newDepth };
        }

        public static void SetPosition(this AnimationLayer layer,
                            int keyFrameIndex,
                            Vector2 newPosition)
        {
            layer.KeyFrames[keyFrameIndex] = new LayerFrameInfo(layer.KeyFrames[keyFrameIndex], layer.KeyFrames[keyFrameIndex].LayerDepth) { Position = newPosition };
        }

        public static void SetTextureFrame(this AnimationLayer layer,
            int keyFrameIndex,
            int textureFrame)
        {
            layer.KeyFrames[keyFrameIndex] = new LayerFrameInfo(layer.KeyFrames[keyFrameIndex], layer.KeyFrames[keyFrameIndex].LayerDepth) { SpriteFrame = textureFrame };
        }

        public static void ToggleVisibility(this AnimationLayer layer,
            int keyFrameIndex)
        {
            bool visible = layer.KeyFrames[keyFrameIndex].IsEnabled;

            visible ^= true;

            layer.KeyFrames[keyFrameIndex] = new LayerFrameInfo(layer.KeyFrames[keyFrameIndex], layer.KeyFrames[keyFrameIndex].LayerDepth) { IsEnabled = visible };
        }
    }
}