using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimated
    {
        IDictionary<string, IAnimation> Animations { get; set; }
    }
}
