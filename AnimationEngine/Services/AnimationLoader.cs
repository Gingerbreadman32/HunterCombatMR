using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Services
{
    public class AnimationLoader
    {
        #region Public Constructors

        public AnimationLoader()
        {
            Containers = new List<ActionContainer>();
        }

        #endregion Public Constructors

        #region Public Properties

        public List<ActionContainer> Containers { get; }

        #endregion Public Properties

        #region Public Methods

        public void LoadContainer(ActionContainer container)
        {
            Containers.Add(container);
        }

        public IAnimation RegisterAnimation(IAnimation action)
        {
            action.Initialize();
            return action;
        }

        public IEnumerable<PlayerAnimation> RegisterAnimations()
        {
            var actions = new List<PlayerAnimation>();
            foreach (var animation in Containers.SelectMany(x => x.AnimatedActions))
            {
                if (!actions.Any(x => x.Name.Equals(animation.Key)))
                {
                    var action = new PlayerAnimation(animation.Key, animation.Value, true);
                    action.Initialize();
                    actions.Add(action);
                }
            }
            return actions;
        }

        public IEnumerable<IAnimation> RegisterAnimations(IEnumerable<IAnimation> loadedActions)
        {
            var actions = new List<IAnimation>();
            foreach (var animation in loadedActions)
            {
                actions.Add(RegisterAnimation(animation));
            }
            return actions;
        }

        #endregion Public Methods
    }
}