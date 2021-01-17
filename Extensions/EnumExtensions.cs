using HunterCombatMR.Attributes;
using System;
using System.Collections.Generic;
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
        #region Public Methods

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

        public static IEnumerable<Enum> GetFlags(this Enum value)
        {
            foreach (Enum flag in Enum.GetValues(value.GetType()))
                if (value.HasFlag(flag))
                    yield return flag;
        }

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

        #endregion Public Methods
    }
}