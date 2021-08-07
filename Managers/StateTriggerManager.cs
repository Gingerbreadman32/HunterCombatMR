using HunterCombatMR.Attributes;
using HunterCombatMR.Constants;
using HunterCombatMR.Services;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HunterCombatMR.Managers
{
    public sealed class StateTriggerManager
        : ManagerBase
    {
        private static IDictionary<Tuple<Type, string>, Func<object, object>> _getPropertyDelegates;
        private static IDictionary<string, Tuple<Type, string>> _triggerParameterDependencies;

        public static object GetComponentProperty(in object componentRef,
            string trigger)
        {
            var componentType = componentRef.GetType();

            if (!_getPropertyDelegates.Any(x => x.Key.Item1 == componentType))
                throw new Exception("No component references exist for this type of component");

            if (!_triggerParameterDependencies.ContainsKey(trigger) || _triggerParameterDependencies[trigger].Item1 != componentType)
                throw new Exception("Trigger parameter dependency does not match current component passed.");

            var propertyName = _triggerParameterDependencies[trigger].Item2;
            var propertyDetails = new Tuple<Type, string>(componentType, propertyName);

            if (!_getPropertyDelegates.Any(x => x.Key.Item2 == propertyName))
                throw new Exception($"No component references exist for property {propertyName} in component type!");

            return _getPropertyDelegates[propertyDetails](componentRef);
        }

        public static Type GetParameterComponentType(string trigger)
        {
            if (!_triggerParameterDependencies.ContainsKey(trigger))
                throw new Exception($"Trigger {trigger} has no saved component dependencies!");

            return _triggerParameterDependencies[trigger].Item1;
        }

        protected override void OnDispose()
        {
            _triggerParameterDependencies = null;
            _getPropertyDelegates = null;
        }

        protected override void OnInitialize()
        {
            _triggerParameterDependencies = new Dictionary<string, Tuple<Type, string>>();
            _getPropertyDelegates = new Dictionary<Tuple<Type, string>, Func<object, object>>();

            GetTriggerParameterData();
        }

        private void GetTriggerParameterData()
        {
            var commonTriggerParameters = ReflectionUtils.GatherConstantsFromStaticType(typeof(CommonTriggerParams));

            foreach (var trigger in commonTriggerParameters.Where(x => x.CustomAttributes.Any()))
            {
                var attributeData = trigger.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(ComponentDependency));
                if (attributeData == null)
                    continue;

                var constantValue = (string)trigger.GetRawConstantValue();

                var ctorParams = attributeData.ConstructorArguments;
                var propertyName = (string)ctorParams.Single(x => x.ArgumentType == typeof(string)).Value;
                var componentType = (Type)ctorParams.Single(x => x.ArgumentType == typeof(Type)).Value;

                PropertyInfo propertyReference = (string.IsNullOrWhiteSpace(propertyName))
                    ? componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance).SingleOrDefault(x => x.CanRead
                        && x.CustomAttributes.Any(y => y.ConstructorArguments.Any(z => z.Value.Equals(constantValue))))
                    : componentType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                if (propertyReference == null)
                    throw new Exception($"No property on required component {componentType.Name} has required attribute to load for trigger parameter {constantValue}!");

                Func<object, object> result = ReflectionUtils.CreateGetMethodDelegate(propertyReference);

                var propertyDetails = new Tuple<Type, string>(componentType,
                    propertyReference.Name);

                _triggerParameterDependencies.Add(constantValue, propertyDetails);
                _getPropertyDelegates.Add(propertyDetails, result);
            }
        }
    }
}