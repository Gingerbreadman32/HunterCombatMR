using HunterCombatMR.Interfaces;

namespace HunterCombatMR.Models
{
    public abstract class Content
        : IContent
    {
        public Content(string internalName)
        {
            InternalName = internalName;
        }

        public string InternalName { get; private set; }

        public bool IsStoredInternally { get; internal set; }

        public abstract IContent CreateNew(string internalName);
    }
}