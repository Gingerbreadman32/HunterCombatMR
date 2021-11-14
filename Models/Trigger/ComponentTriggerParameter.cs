using HunterCombatMR.Interfaces;
using HunterCombatMR.Managers;

namespace HunterCombatMR.Models.Trigger
{
    public struct ComponentTriggerParameter
        : IScriptElement<string>
    {
        public ComponentTriggerParameter(string parameter)
        {
            Parameter = parameter;
        }

        public string Parameter { get; }

        public string Solve(int entityId = -1)
        {
            var componentType = TriggerFunctionManager.GetParameterComponentType(Parameter);

            if (!ComponentManager.HasComponent(entityId, componentType))
                return "";

            ref readonly var component = ref ComponentManager.GetEntityComponent(entityId, componentType);
            return TriggerFunctionManager.GetComponentProperty(in component, Parameter).ToString();
        }

        public override string ToString()
            => Parameter;
    }
}