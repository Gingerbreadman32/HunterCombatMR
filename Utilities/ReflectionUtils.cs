using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HunterCombatMR.Utilities
{
    internal static class ReflectionUtils
    {
        private const string InnerDelegateName = "CallInnerDelegate";
        internal static IEnumerable<FieldInfo> GatherConstantsFromStaticType(Type staticType)
            => staticType.GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.IsLiteral && !x.IsInitOnly);

        internal static Func<object, object> CallInnerDelegate<TClass, TResult>(Func<TClass, TResult> delegateMethod)
            => instance => delegateMethod((TClass)instance);

        internal static Func<object, object> CallInnerDelegateStatic<TResult>(Func<TResult> delegateMethod)
            => instance => delegateMethod();

        internal static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            var member = expression.Body as MethodCallExpression;

            if (member != null)
                return member.Method;

            throw new ArgumentException("Expression is not a method", "expression");
        }

        internal static IEnumerable<T> InstatiateTypesFromInterface<T>()
        {
            var types = new List<T>();
            foreach (var type in typeof(T).Assembly.GetTypes().Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract))
            {
                types.Add((T)Activator.CreateInstance(type));
            }
            return types;
        }

        // Will need to make set version of this later
        internal static Func<object, object> CreateGetMethodDelegate(PropertyInfo propertyReference) 
        {
            try
            {
                var resultType = propertyReference.PropertyType;
                var declaringType = propertyReference.DeclaringType;
                var isStatic = propertyReference.GetMethod.IsStatic;

                Delegate getMethodDelegate = CreateGetMethodDelegate(propertyReference, resultType, declaringType, isStatic);
                MethodInfo genericMethodInfo = CreateGenericMethodInfo(resultType, declaringType, isStatic);

                return (Func<object, object>)genericMethodInfo.Invoke(null, new[] { getMethodDelegate });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static MethodInfo CreateGenericMethodInfo(Type resultType, 
            Type declaringType, 
            bool isStatic)
        {
            var innerDelegateName = (isStatic) 
                ? InnerDelegateName + "Static" 
                : InnerDelegateName;

            var genericMethod = typeof(ReflectionUtils).GetMethod(innerDelegateName, BindingFlags.Static | BindingFlags.NonPublic);
                
            var genericMethodInfo = (isStatic) 
                ? genericMethod.MakeGenericMethod(resultType)
                : genericMethod.MakeGenericMethod(declaringType, resultType);

            return genericMethodInfo;
        }

        private static Delegate CreateGetMethodDelegate(PropertyInfo propertyReference, 
            Type resultType, 
            Type declaringType, 
            bool isStatic)
        {
            ParameterExpression expressionParameter = (!isStatic) 
                ? Expression.Parameter(declaringType) 
                : null;
            MemberExpression propertyExpression = Expression.Property(expressionParameter, propertyReference);        

            if (!isStatic)
            {
                return Expression.Lambda(Expression.GetFuncType(declaringType, resultType),
                        propertyExpression,
                        expressionParameter).Compile();
            }

            return Expression.Lambda(Expression.GetFuncType(resultType), propertyExpression).Compile();
        }
    }
}