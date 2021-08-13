using System.Collections.Generic;
using Terraria.ModLoader;

namespace HunterCombatMR.Messages.AnimationSystem
{
    public class CreatePlayerLayersMessage
    {
        private List<PlayerLayer> _playerLayers;

        public CreatePlayerLayersMessage(int entityId,
            ref List<PlayerLayer> playerLayers)
        {
            EntityId = entityId;
            PlayerLayers = playerLayers;
        }

        public int EntityId { get; }
        public ref List<PlayerLayer> PlayerLayers { get => ref _playerLayers; }
    }
}