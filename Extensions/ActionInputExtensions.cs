using HunterCombatMR.Enumerations;
using HunterCombatMR.Utilities;
using System.Collections.Generic;
using Terraria.GameInput;

namespace HunterCombatMR.Extensions
{
    public static class ActionInputExtensions
    {
        public static string GetGameCommand(this ActionInputs input)
        {
            var attribute = GameCommandCache.GetGameCommand(input.ToString());
            return attribute?.GetName() ?? "";
        }

        private static bool CheckKeyStatus(ActionInputs comboInput,
            IDictionary<string, bool> triggerSet)
        {
            bool result;
            if (!triggerSet.TryGetValue(GetGameCommand(comboInput), out result))
                return false;
            return result;
        }

        public static bool IsMouse(this ActionInputs comboInput)
             => GetGameCommand(comboInput).ContainsIgnoreCase("mouse");

        public static bool IsPressed(this ActionInputs comboInput)
            => CheckKeyStatus(comboInput, PlayerInput.Triggers.Current.KeyStatus);

        public static bool JustPressed(this ActionInputs comboInput)
             => CheckKeyStatus(comboInput, PlayerInput.Triggers.JustPressed.KeyStatus);

        public static bool JustReleased(this ActionInputs comboInput)
             => CheckKeyStatus(comboInput, PlayerInput.Triggers.JustReleased.KeyStatus);
    }
}