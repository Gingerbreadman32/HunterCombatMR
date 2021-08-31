using HunterCombatMR.Enumerations;
using HunterCombatMR.Managers;
using System.Collections.Generic;
using Terraria.GameInput;

namespace HunterCombatMR.Extensions
{
    public static class ActionInputExtensions
    {
        public static string GetGameCommand(this DefinedInputs input)
        {
            var attribute = GameCommandManager.GetGameCommand(input.ToString());
            return attribute?.GetName() ?? "";
        }

        private static bool CheckKeyStatus(DefinedInputs comboInput,
            IDictionary<string, bool> triggerSet)
        {
            bool result;
            if (!triggerSet.TryGetValue(GetGameCommand(comboInput), out result))
                return false;
            return result;
        }

        public static bool IsMouse(this DefinedInputs comboInput)
             => GetGameCommand(comboInput).ContainsIgnoreCase("mouse");

        public static bool IsPressed(this DefinedInputs comboInput)
            => CheckKeyStatus(comboInput, PlayerInput.Triggers.Current.KeyStatus);

        public static bool JustPressed(this DefinedInputs comboInput)
             => CheckKeyStatus(comboInput, PlayerInput.Triggers.JustPressed.KeyStatus);

        public static bool JustReleased(this DefinedInputs comboInput)
             => CheckKeyStatus(comboInput, PlayerInput.Triggers.JustReleased.KeyStatus);
    }
}