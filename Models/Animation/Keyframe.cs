namespace HunterCombatMR.Models.Animation
{
    /// <summary>
    /// Object for storing keyframe info
    /// </summary>
    public struct Keyframe
        : IKeyframe
    {
        public Keyframe(FrameLength length)
        {
            Frames = length;
        }

        /// <summary>
        /// The amount of frames/ticks the game will display the given keyframe
        /// </summary>
        public FrameLength Frames { get; set; }
    }
}