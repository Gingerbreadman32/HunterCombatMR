using System;

namespace HunterCombatMR.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    internal sealed class TexturePath
        : Attribute
    {
        public string TexurePathString;

        public TexturePath(string path)
        {
            TexurePathString = path;
        }

        public string GetName()
            => TexurePathString;
    }
}