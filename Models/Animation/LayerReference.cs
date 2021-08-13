namespace HunterCombatMR.Models.Animation
{
    public struct LayerReference
    {
        public LayerReference(Layer layer, 
            LayerData frameData, 
            FrameIndex index, 
            int? depth = null)
        {
            Layer = layer;
            FrameData = frameData;
            Index = index;
            CurrentDepth = (depth == null || !depth.HasValue) ? layer.Depth : depth.Value;
        }

        public LayerData FrameData { get; }
        public FrameIndex Index { get; }
        public Layer Layer { get; }

        public int CurrentDepth { get; }
    }
}