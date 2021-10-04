using System;

namespace HunterCombatMR.Attributes
{
    public sealed class ManagerDependency
        : Attribute
    {
        public ManagerDependency(Type manager)
        {
            Manager = manager;
        }

        public Type Manager { get; }
    }
}