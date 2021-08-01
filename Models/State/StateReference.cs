namespace HunterCombatMR.Models.State
{
    public struct StateReference
    {
        public StateReference(int id,
            string name)
        {
            StateNumber = id;
            StateName = name;
        }

        public int StateNumber { get; }

        public string StateName { get; }
    }
}