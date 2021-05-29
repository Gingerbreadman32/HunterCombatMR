using HunterCombatMR.Enumerations;
using HunterCombatMR.Utilities;
using Terraria.GameInput;

namespace HunterCombatMR.Extensions
{
    public static class ActionInputExtensions
    {
        public static string GetGameCommand(this ActionInputs input)
        {
            var attribute = CachingUtils.GetGameCommand(input.ToString());
            return attribute?.GetName() ?? "";
        }

        public static bool IsMouse(this ActionInputs comboInput)
             => GetGameCommand(comboInput).ContainsIgnoreCase("mouse");

        public static bool IsPressed(this ActionInputs comboInput)
                     => PlayerInput.Triggers.Current.KeyStatus[GetGameCommand(comboInput)];

        public static bool JustPressed(this ActionInputs comboInput)
             => PlayerInput.Triggers.JustPressed.KeyStatus[GetGameCommand(comboInput)];

        public static bool JustReleased(this ActionInputs comboInput)
             => PlayerInput.Triggers.JustReleased.KeyStatus[GetGameCommand(comboInput)];
    }
}