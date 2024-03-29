﻿using HunterCombatMR.Attributes;
using System;

namespace HunterCombatMR.Enumerations
{
    /// <summary>
    /// Enumeration for storing multiple active inputs.
    /// </summary>
    [Flags]
    public enum DefinedInputs
    {
        NoInput = 0,
        [GameCommand("MouseLeft")]
        PrimaryAction = 1,
        [GameCommand("MouseRight")]
        SecondaryAction = 2,
        BothActions = 3,
        [GameCommand("Jump")]
        Jump = 4,
        [GameCommand("Grapple")]
        Hook = 8,
        [GameCommand("Right")]
        Right = 16,
        [GameCommand("Left")]
        Left = 32,
        [GameCommand("Up")]
        Up = 64,
        [GameCommand("Down")]
        Down = 128
    }
}