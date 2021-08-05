using HunterCombatMR.Constants;
using HunterCombatMR.Managers;
using HunterCombatMR.Models.Components;
using HunterCombatMR.Utilities;
using Terraria;

namespace HunterCombatMR.Models.Systems
{
    public class EntityStateSystem
        : ModSystem<EntityStateComponent>
    {
        public override void PostEntityUpdate()
        {
            foreach (var entity in ReadEntities())
            {
                ref readonly var component = ref ComponentManager.GetEntityComponent<EntityStateComponent>(entity);

                if (InputCheckingUtils.PlayerInputBufferPaused())
                    continue;
                    
               component.CurrentStateInfo.Time++;
            }
        }
    }
}