using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Utilities;
using System.Collections.Generic;

namespace HunterCombatMR.Models.Components
{
    public class InputComponent
        : ModComponent
    {
        public InputComponent() 
            : base()
        {
            BufferedInputs = new Queue<BufferedInput>();
            IgnoreBlockInput = true;
            Player = -1;
        }

        public Queue<BufferedInput> BufferedInputs { get; set; }

        public bool IgnoreBlockInput { get; set; }

        public int Player { get; set; }
    }
}