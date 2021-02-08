using HunterCombatMR.Enumerations;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct BufferedInput
    {
        #region Private Fields

        private const int _maxFrameBuffer = 60;

        #endregion Private Fields

        #region Public Constructors

        public BufferedInput(ComboInputs input,
            int framesBuffered = 0,
            int maxBufferFrames = _maxFrameBuffer)
        {
            Input = input;
            FramesSinceBuffered = framesBuffered;
            MaximumBufferFrames = maxBufferFrames;
        }

        #endregion Public Constructors

        #region Public Properties

        public int FramesSinceBuffered { get; set; }
        public ComboInputs Input { get; set; }
        public int MaximumBufferFrames { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void AddFramestoBuffer(int amount)
        {
            FramesSinceBuffered += amount;
        }

        #endregion Public Methods
    }
}