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
        /// <summary>
        /// Gets the description of the specified enumeration
        /// </summary>
        /// <param name="value">The enum value</param>
        /// <returns>The description if it exists; otherwise, the string value of the enumeration.</returns>
        public static string GetDescription(this Enum value)
            => value.GetAttribute<DescriptionAttribute>()
            ?.Description
            ?? value.ToString();

        public static IEnumerable<Enum> GetFlags(this Enum value)
            => Enum.GetValues(value.GetType()).OfType<Enum>().Where(x => value.HasFlag(x));

        /// <summary>
        /// Gets the texture path of the specified enumeration
        /// </summary>
        /// <param name="value">The enum value</param>
        /// <returns>The texture path if it exists; otherwise, null.</returns>
        public static string GetTexturePath(this Enum value)
            => value.GetAttribute<TexturePath>()
            ?.TexurePathString
            ?? null;

        /// <summary>
        /// Gets the description of the specified enumeration
        /// </summary>
        /// <param name="value">The enum value</param>
        /// <returns>The description if it exists; otherwise, the string value of the enumeration.</returns>
        public static T GetAttribute<T>(this Enum value)
            where T : Attribute
            => value
            .GetType()
            .GetMember(value.ToString())
            .SingleOrDefault()
            ?.GetCustomAttribute<T>();


        public static bool Subset(this Enum @value,
                Enum input)
        {
            if (@value.GetType() != input.GetType())
                return false;

            if (Convert.ToInt64(input).Equals(0))
            {
                if (!Convert.ToInt64(@value).Equals(0))
                    return false;

                return true;
            }

            return input.GetFlags().Where(num => !Convert.ToInt64(num).Equals(0)).All(num => @value.HasFlag(num));
        }
    }
}