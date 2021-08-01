﻿using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.Models.Components
{
    public struct InputComponent
    {
        public InputComponent(int player,
            bool ignoreBlockInput = true)
        {
            BufferedInputs = new Queue<BufferedInput>();
            IgnoreBlockInput = ignoreBlockInput;
            Player = player;
        }

        public Queue<BufferedInput> BufferedInputs { get; set; }

        public bool IgnoreBlockInput { get; set; }

        public int Player { get; set; }
    }
}