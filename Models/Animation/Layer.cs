using System.Collections.Generic;

namespace HunterCombatMR.Models.Animation
{
    public struct Layer
    {
        public Layer(string name,
            IEnumerable<LayerData> layerData)
        {
            Name = name;
            LayerData = layerData;
        }

        public IEnumerable<LayerData> LayerData { get; }

        /// <summary>
        /// Name of the layer
        /// </summary>
        public string Name { get; }
    }
}