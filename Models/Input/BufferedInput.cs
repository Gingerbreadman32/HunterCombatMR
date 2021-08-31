using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;

namespace HunterCombatMR.Models.Input
{
    public class BufferedInput
    {
        private const int _defaultMaxFrameBuffer = 60;
        private bool _markedForDelete;
        private bool _held;
        private int _framesHeld;
        private int _framesSinceBuffered;
        private DefinedInputs _input;

        public BufferedInput(DefinedInputs input)
        {
            _input = input;
            _held = true;
        }

        public int FramesHeld { get => _framesHeld; set { _framesHeld = value; } }
        public int FramesSinceBuffered { get => _framesSinceBuffered; set { _framesSinceBuffered = value; } }
        public DefinedInputs Input { get => _input; }
        public FrameLength MaximumBufferFrames { get => _defaultMaxFrameBuffer; }

        public bool MarkedForDeletion { get => _markedForDelete; }

        public void Reset()
        {
            _markedForDelete = true;
        }

        public void Update()
        {
            FramesSinceBuffered++;
            if (Input.IsPressed() && _held)
                FramesHeld++;

            if (!Input.IsPressed())
                _held = false;

            if (FramesSinceBuffered > MaximumBufferFrames && !_held)
                Reset();
        }
    }
}