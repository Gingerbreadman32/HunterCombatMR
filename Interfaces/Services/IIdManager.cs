namespace HunterCombatMR.Interfaces.Services
{
    public interface IIdManager
    {
        void Free(int id);
        int NextID();
    }
}