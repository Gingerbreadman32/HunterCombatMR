using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Interfaces;
using System;

namespace HunterCombatMR.Utilities
{
    public static class ContentUtils
    {
        public static T Get<T>(string name) where T : IHunterCombatContentInstance
                    => (T)GetInstance<T>(name).CloneFrom(name);

        public static T Get<T>(T instance) where T : IHunterCombatContentInstance
                    => (T)instance.CloneFrom(instance.InternalName);

        /// <summary>
        /// Gets the original instance of a specified content type.
        /// </summary>
        /// <typeparam name="T">The content type</typeparam>
        /// <param name="name">Internal name of the instance requested</param>
        /// <returns>
        /// The original reference instance of the specified named content type,
        /// returns null if a content instance with that name does not exist.
        /// </returns>
        /// <remarks>
        /// ONLY use this for modification, otherwise use GetNew.
        /// </remarks>
        internal static T GetInstance<T>(string name) where T : IHunterCombatContentInstance
            => HunterCombatMR.Instance.Content.GetContentInstance<T>(name);
    }
}