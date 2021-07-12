using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;

namespace HunterCombatMR.AttackEngine.Models
{
    public class BufferedInput
    {
        private const int _defaultMaxFrameBuffer = 60;

        public BufferedInput()
        {
            Input = ActionInputs.NoInput;
        }

        public BufferedInput(ActionInputs input)
        {
            Input = input;
        }

        public int FramesHeld { get; set; }
        public int FramesSinceBuffered { get; set; }
        public ActionInputs Input { get; set; }
        public int MaximumBufferFrames { get => _defaultMaxFrameBuffer; }

        public void Reset()
        {
            Input = ActionInputs.NoInput;
            FramesHeld = 0;
            FramesSinceBuffered = 0;
        }

        public void Update()
        {
            FramesSinceBuffered++;
            if (Input.IsPressed())
                FramesHeld++;

            if (FramesSinceBuffered > MaximumBufferFrames)
                Reset();
        }
    }
}