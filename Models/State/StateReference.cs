namespace HunterCombatMR.Models.State
{
    public struct StateReference
    {
        public StateReference(int id,
            string name)
        {
            StateId = id;
            StateName = name;
        }

        public int StateId { get; }

        public string StateName { get; }
    }
}