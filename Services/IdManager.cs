using HunterCombatMR.Interfaces.Services;
using System.Collections.Generic;

namespace HunterCombatMR.Services
{
    public class IdManager
        : IIdManager
    {
        private readonly Stack<int> _availableIDs = new Stack<int>();
        private int _nextID = 0;

        public void Free(int id)
        {
            _availableIDs.Push(id);
        }

        public int NextID()
        {
            if (_availableIDs.Count > 0)
            {
                return _availableIDs.Pop();
            }

            return _nextID++;
        }
    }
}