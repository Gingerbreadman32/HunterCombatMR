namespace HunterCombatMR.Models.State
{
    public class StateControllerInfo
    {
        public StateControllerInfo(StateController controller,
            int timesFired = 0)
        {
            Definition = controller;
            TimesFired = timesFired;
        }

        public StateController Definition { get; set; }

        public int TimesFired { get; set; }
    }
}