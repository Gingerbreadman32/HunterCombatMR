using Terraria;
using Terraria.ModLoader;

namespace HunterCombatMR.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="ModProjectile"/> class
    /// </summary>
    public static class ModProjectileExtensions
    {
        /// <summary>
        /// Just a copy "constructor" overload for projectiles
        /// </summary>
        /// <param name="copyProjectile">Pre-made projectile you want to create</param>
        /// <returns>The Projectile instance</returns>
        public static Projectile NewProjectileDirect(this ModProjectile copyProjectile)
            => Main.projectile[Projectile.NewProjectile(copyProjectile.projectile.position.X,
                                    copyProjectile.projectile.position.Y,
                                    copyProjectile.projectile.velocity.X,
                                    copyProjectile.projectile.velocity.Y,
                                    copyProjectile.projectile.type,
                                    copyProjectile.projectile.damage,
                                    copyProjectile.projectile.knockBack,
                                    copyProjectile.projectile.owner,
                                    copyProjectile.projectile.ai[0],
                                    copyProjectile.projectile.ai[1])];

        /// <summary>
        /// Just a copy "constructor" overload for projectiles
        /// </summary>
        /// <param name="copyProjectile">Pre-made projectile you want to create</param>
        /// <returns>The Projectile type int</returns>
        public static int NewProjectile(this ModProjectile copyProjectile)
            => Projectile.NewProjectile(copyProjectile.projectile.position.X,
                                    copyProjectile.projectile.position.Y,
                                    copyProjectile.projectile.velocity.X,
                                    copyProjectile.projectile.velocity.Y,
                                    copyProjectile.projectile.type,
                                    copyProjectile.projectile.damage,
                                    copyProjectile.projectile.knockBack,
                                    copyProjectile.projectile.owner,
                                    copyProjectile.projectile.ai[0],
                                    copyProjectile.projectile.ai[1]);
    }
}