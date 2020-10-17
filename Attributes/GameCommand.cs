using System;

namespace HunterCombatMR.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public sealed class GameCommand
        : Attribute
    {
        public string KeyInputName;

        public GameCommand(string keyName)
        {
            KeyInputName = keyName;
        }

        public string GetName()
            => KeyInputName;
    }
}