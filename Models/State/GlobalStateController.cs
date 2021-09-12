namespace HunterCombatMR.Models.State
{
    public struct GlobalStateController
    {
        public GlobalStateController(string name,
            StateController controller,
            int priority)
        {
            Name = name;
            Controller = controller;
            Priority = priority;
        }
        public string Name { get; }
        public StateController Controller { get; }
        public int Priority { get; }
    }
}