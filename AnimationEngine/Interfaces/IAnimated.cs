using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimated
    {
        IAnimation Animation { get; }

        void Play();

        void Stop();

        void Pause();
    }
}
