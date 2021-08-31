using HunterCombatMR.Enumerations;

namespace HunterCombatMR.Models.Input
{
    public class HeldInput
    {
        public HeldInput(DefinedInputs input,
            int framesHeld = 0)
        {
            Input = input;
            FramesHeld = framesHeld;
        }

        public int FramesHeld { get; set; }
        public DefinedInputs Input { get; set; }
    }
}