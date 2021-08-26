using System.Collections.Generic;

namespace HunterCombatMR.Models.Animation.Entity
{
    public struct EntityAnimationLayer
    {
        public EntityAnimationLayer(string name,
            IEnumerable<EntityAnimationLayerData> layerData)
        {
            Name = name;
            LayerData = layerData;
        }

        public IEnumerable<EntityAnimationLayerData> LayerData { get; }

        /// <summary>
        /// Name of the layer
        /// </summary>
        public string Name { get; }
    }
}