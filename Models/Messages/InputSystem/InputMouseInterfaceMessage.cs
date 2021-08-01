﻿namespace HunterCombatMR.Models.Messages.InputSystem
{
    public struct InputMouseInterfaceMessage
    {
        public InputMouseInterfaceMessage(bool mouseInterface)
        {
            MouseInterface = mouseInterface;
        }

        public bool MouseInterface { get; }
    }
}