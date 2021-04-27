using HunterCombatMR.Enumerations;

namespace HunterCombatMR.AttackEngine.Models
{
    public class HeldInput
    {
        public HeldInput(ActionInputs input,
            int framesHeld = 0)
        {
            Input = input;
            FramesHeld = framesHeld;
        }

        public int FramesHeld { get; set; }
        public ActionInputs Input { get; set; }
    }
}