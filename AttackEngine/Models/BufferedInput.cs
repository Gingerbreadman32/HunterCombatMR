using HunterCombatMR.Enumerations;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class BufferedInput
    {
        private const int _maxFrameBuffer = 60;

        private const int _maxInputInstances = 3;

        public ComboInputs Input { get; set; }

        public int FramesSinceBuffered { get; set; }

        public int MaximumBufferFrames { get; set; }

        public int MaximumBufferInstances { get; set; }

        public BufferedInput(ComboInputs input,
            int framesBuffered = 0,
            int maxBufferFrames = _maxFrameBuffer,
            int maxInstances = _maxInputInstances)
        {
            Input = input;
            FramesSinceBuffered = framesBuffered;
            MaximumBufferFrames = maxBufferFrames;
            MaximumBufferInstances = maxInstances;
        }
    }
}