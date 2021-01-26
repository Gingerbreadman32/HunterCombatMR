using HunterCombatMR.AnimationEngine.Interfaces;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class ActionBase<TObject, TAnimationType> where TObject
        : IAnimated<TAnimationType> where TAnimationType
        : Animation
    {
        public IEnumerable<TAnimationType> Animations { get; protected set; }

        public TObject ActionObject { get; protected set; }
    }
}