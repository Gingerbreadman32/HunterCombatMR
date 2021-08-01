using HunterCombatMR.Services;
using System.Collections.Generic;

namespace HunterCombatMR.Managers
{
    internal sealed class IdManager
        : ManagerBase
    {
        private static Stack<int> _availableIDs;
        private static int _nextID;

        protected override void OnDispose()
        {
            _availableIDs = null;
        }

        protected override void OnInitialize()
        {
            _availableIDs = new Stack<int>();
        }

        internal static void Free(int id)
        {
            _availableIDs.Push(id);
        }

        internal static int NextID()
        {
            if (_availableIDs.Count > 0)
            {
                return _availableIDs.Pop();
            }

            return _nextID++;
        }
    }
}