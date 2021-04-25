using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Seeds.Animations;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Services
{
    internal sealed class AnimationLoader
    {
        internal AnimationLoader()
        {
            Containers = new List<AnimationSeed>();
        }

        internal List<AnimationSeed> Containers { get; }

        internal void LoadContainer(AnimationSeed container)
        {
            Containers.Add(container);
        }

        internal IAnimation RegisterAnimation(IAnimation action)
        {
            action.Initialize();
            return action;
        }

        internal IEnumerable<PlayerAnimation> RegisterAnimations()
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

        internal IEnumerable<IAnimation> RegisterAnimations(IEnumerable<IAnimation> loadedActions)
        {
            var actions = new List<IAnimation>();
            foreach (var animation in loadedActions)
            {
                actions.Add(RegisterAnimation(animation));
            }
            return actions;
        }
    }
}