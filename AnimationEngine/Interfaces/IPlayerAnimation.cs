using System.Collections.Generic;
using Terraria.ModLoader;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IPlayerAnimation
    {
        List<PlayerLayer> DrawPlayerLayers(List<PlayerLayer> layers);
    }
}