using System;

namespace HunterCombatMR.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ComponentDependency
        : Attribute
    {
        public ComponentDependency(Type componentType,
            string property = "")
        {
            ComponentType = componentType;
            PropertyName = property;
        }

        public Type ComponentType { get; }

        public string PropertyName { get; }
    }
}