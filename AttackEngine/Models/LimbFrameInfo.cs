using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AttackEngine.Models
{
    public struct LimbFrameInfo
    {
        public int SpriteFrame { get; set; }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public int LayerDepth { get; set; }
    }
}
