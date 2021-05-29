using HunterCombatMR.Interfaces;
using HunterCombatMR.Models;
using HunterCombatMR.Seeds.Animations;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Services
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

        internal ICustomAnimation RegisterAnimation(ICustomAnimation action)
        {
            action.Initialize();
            return action;
        }

        internal ICollection<PlayerAnimation> RegisterAnimations()
        {
            var actions = new List<PlayerAnimation>();
            foreach (var animation in Containers.SelectMany(x => x.AnimatedActions))
            {
                if (!actions.Any(x => x.DisplayName.Equals(animation.Key)))
                {
                    var action = new PlayerAnimation(animation.Key, animation.Value, true);
                    action.Initialize();
                    actions.Add(action);
                }
            }
            return actions;
        }

        internal ICollection<ICustomAnimation> RegisterAnimations(IEnumerable<ICustomAnimation> loadedActions)
        {
            var actions = new List<ICustomAnimation>();
            foreach (var animation in loadedActions)
            {
                actions.Add(RegisterAnimation(animation));
            }
            return actions;
        }
    }
}