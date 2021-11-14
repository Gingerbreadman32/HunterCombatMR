using System;

namespace HunterCombatMR.Models
{
    public struct ComponentProperty
    {
        public ComponentProperty(Type componentType,
            string name)
        {
            ComponentType = componentType;
            PropertyName = name;
        }

        public Type ComponentType { get; }

        public string PropertyName { get; }
    }
}