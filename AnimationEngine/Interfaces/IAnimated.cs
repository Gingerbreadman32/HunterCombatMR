using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;

namespace HunterCombatMR.AnimationEngine.Interfaces
{
    public interface IAnimated
    {
        #region Public Properties

        string Name { get; }

        KeyFrameProfile KeyFrameProfile { get; }

        IDictionary<string, string> DefaultParameters { get; }

        #endregion Public Properties

        #region Public Methods

        void Initialize();

        void Update(IAnimator animator);

        #endregion Public Methods
    }
}