using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HunterCombatMR.Utilities
{
    internal static class ReflectionUtils
    {
        internal static IEnumerable<FieldInfo> GatherConstantsFromStaticType(Type staticType)
            => staticType.GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.IsLiteral && !x.IsInitOnly);

        internal static Func<object, object> CallInnerDelegate<TClass, TResult>(Func<TClass, TResult> delegateMethod)
            => instance => delegateMethod((TClass)instance);

        internal static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            var member = expression.Body as MethodCallExpression;

            if (member != null)
                return member.Method;

            throw new ArgumentException("Expression is not a method", "expression");
        }
    }
}