using HunterCombatMR.AnimationEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AttackEngine.Models
{
    public class PlayerAttackAnimationInfo
        : IAnimated
    {
        public List<IDictionary<int, LimbFrameInfo>> LimbsInfo { get; }
        public IAnimation Animation { get; }

        public PlayerAttackAnimationInfo(IAnimation animation)
        {
            Animation = animation;
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
