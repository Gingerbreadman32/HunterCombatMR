using HunterCombatMR.Enumerations;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct BufferedInput
    {
        private const int _defaultMaxFrameBuffer = 5;

        public BufferedInput(ActionInputs input,
            int framesBuffered = 0,
            int maxBufferFrames = _defaultMaxFrameBuffer)
        {
            Input = input;
            FramesSinceBuffered = framesBuffered;
            MaximumBufferFrames = maxBufferFrames;
        }

        public int FramesSinceBuffered { get; set; }
        public ActionInputs Input { get; set; }
        public int MaximumBufferFrames { get; set; }

        public void AddFramestoBuffer(int amount)
        {
            FramesSinceBuffered += amount;
        }
    }
}