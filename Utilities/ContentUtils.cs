using HunterCombatMR.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Utilities
{
    public static class ContentUtils
    {
        public static T Get<T>(string name) where T : IContent
                    => (T)GetInstance<T>(name).CreateNew(name);

        public static IDictionary<string, Texture2D> GetTexturesFromPath(string path)
            => HunterCombatMR.Instance.VariableTextures.Where(x => x.Key.StartsWith(path)).ToDictionary(x => x.Key, y => y.Value);

        public static bool TryGet<T>(string name, out T instance) where T : IContent
        {
            bool exists = HunterCombatMR.Instance.Content.CheckContentInstanceByName<T>(name);
            instance = default(T);

            if (exists)
                instance = (T)GetInstance<T>(name).CreateNew(name);

            return exists;
        }

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
        internal static T GetInstance<T>(string name) where T : IContent
            => HunterCombatMR.Instance.Content.GetContentInstance<T>(name);
    }
}