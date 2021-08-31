using Microsoft.Xna.Framework;

namespace HunterCombatMR.Models
{
    public class Hitbox
    {
        public int[] FramesActive { get; set; }

        public Rectangle HitboxArea { get; set; }

        public float KnockbackAmount { get; set; }

        public float KnockbackDirection { get; set; }

        public float Damage { get; set; }

        public Hitbox(int startFrame,
            int endFrame,
            Rectangle area,
            float damage,
            float knockbackAmount,
            float knockbackDirection)
        {
            int frameAmount = endFrame - startFrame;
            FramesActive = new int[frameAmount];

            for (var f = 0; f < frameAmount; f++)
            {
                FramesActive[f] = startFrame + f;
            }
            HitboxArea = area;
            Damage = damage;
            KnockbackAmount = knockbackAmount;
            KnockbackDirection = knockbackDirection;
        }

        public Hitbox(int[] framesActive,
            Rectangle area,
            float damage,
            float knockbackAmount,
            float knockbackDirection)
        {
            FramesActive = framesActive;
            HitboxArea = area;
            Damage = damage;
            KnockbackAmount = knockbackAmount;
            KnockbackDirection = knockbackDirection;
        }
    }
}