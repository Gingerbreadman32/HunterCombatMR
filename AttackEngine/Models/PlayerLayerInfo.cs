using HunterCombatMR.AnimationEngine.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AttackEngine.Models
{
    /// <summary>
    /// Information pertaining to custom player layers that the <see cref="PlayerAttackAnimation"/> will reference for each layer.
    /// </summary>
    public class PlayerLayerInfo
    {
        public string Name { get; set; }

        public Rectangle StartFrameRect { get; set; }

        public Vector2 StartOffset { get; set; }
    }
}
