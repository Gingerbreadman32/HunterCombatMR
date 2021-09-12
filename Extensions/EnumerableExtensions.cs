using System;
using System.Collections.Generic;

namespace HunterCombatMR.Extensions
{
    public static class EnumerableExtensions
    {
        public delegate Boolean TryFunc<T, TOut>(T input, out TOut value);

        public static IEnumerable<TOut> SelectValuesWhere<T, TOut>(this IEnumerable<T> source, TryFunc<T, TOut> tryFunc)
        {
            foreach (T item in source)
            {
                if (tryFunc(item, out TOut value))
                {
                    yield return value;
                }
            }
        }

        public static IEnumerable<KeyValuePair<T, TOut>> SelectWhere<T, TOut>(this IEnumerable<T> source, TryFunc<T, TOut> tryFunc)
        {
            foreach (T item in source)
            {
                if (tryFunc(item, out TOut value))
                {
                    yield return new KeyValuePair<T, TOut>(item, value);
                }
            }
        }
    }
}