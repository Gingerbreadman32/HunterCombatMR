using HunterCombatMR.Enumerations;
using HunterCombatMR.Attributes;
using System;
using System.Reflection;
using Terraria.GameInput;

namespace HunterCombatMR.Extensions
{
    public static class ComboInputExtentions
    {
        public static string GetGameCommand(this ComboInputs comboInput)
        {
            var attribute = Attribute.GetCustomAttribute(comboInput.GetType().GetField(comboInput.ToString()), typeof(GameCommand)) as GameCommand;
            return attribute.GetName();
        }

        public static bool IsPressed(this ComboInputs comboInput)
             => PlayerInput.Triggers.Current.KeyStatus[GetGameCommand(comboInput)];

        public static bool JustPressed(this ComboInputs comboInput)
             => PlayerInput.Triggers.JustPressed.KeyStatus[GetGameCommand(comboInput)];

        public static bool JustReleased(this ComboInputs comboInput)
             => PlayerInput.Triggers.JustReleased.KeyStatus[GetGameCommand(comboInput)];
    }
}