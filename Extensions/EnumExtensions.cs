using HunterCombatMR.Attributes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace HunterCombatMR.Extensions
{
    /// <summary>
    /// Extension methods for enumerations.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description of the specified enumeration
        /// </summary>
        /// <param name="value">The enum value</param>
        /// <returns>The description if it exists; otherwise, the string value of the enumeration.</returns>
        public static string GetDescription(this Enum value)
            => value
            .GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DescriptionAttribute>()
            ?.Description
            ?? value.ToString();

        /// <summary>
        /// Gets the texture path of the specified enumeration
        /// </summary>
        /// <param name="value">The enum value</param>
        /// <returns>The texture path if it exists; otherwise, null.</returns>
        public static string GetTexturePath(this Enum value)
            => value
            .GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<TexturePath>()
            ?.TexurePathString
            ?? null;
    }
}