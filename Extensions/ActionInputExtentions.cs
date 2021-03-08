using HunterCombatMR.Attributes;
using HunterCombatMR.Enumerations;
using System;
using Terraria.GameInput;

namespace HunterCombatMR.Extensions
{
    public static class ActionInputExtentions
    {
        #region Public Methods

        public static string GetGameCommand(this ActionInputs input)
        {
            // @@warn Cache this reflection somewhere (will probably need a instance cache or something)
            var attribute = Attribute.GetCustomAttribute(input.GetType().GetField(input.ToString()), typeof(GameCommand)) as GameCommand;
            return attribute?.GetName() ?? "";
        }

        public static bool HasGameCommand(this ActionInputs input)
                    => Attribute.IsDefined(input.GetType().GetField(input.ToString()), typeof(GameCommand));

        public static bool IsPressed(this ActionInputs comboInput)
             => PlayerInput.Triggers.Current.KeyStatus[GetGameCommand(comboInput)];

        public static bool JustPressed(this ActionInputs comboInput)
             => PlayerInput.Triggers.JustPressed.KeyStatus[GetGameCommand(comboInput)];

        public static bool JustReleased(this ActionInputs comboInput)
             => PlayerInput.Triggers.JustReleased.KeyStatus[GetGameCommand(comboInput)];

        #endregion Public Methods
    }
}