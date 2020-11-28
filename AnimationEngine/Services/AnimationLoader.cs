using HunterCombatMR.AnimationEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Services
{
    public class AnimationLoader
    {
        public List<ActionContainer> Containers { get; }

        public AnimationLoader()
        {
            Containers = new List<ActionContainer>();
        }

        public void LoadContainer(ActionContainer container)
        {
            Containers.Add(container);
        }

        public IEnumerable<PlayerActionAnimation> RegisterAnimations()
        {
            var actions = new List<PlayerActionAnimation>();
            foreach (var animation in Containers.SelectMany(x => x.AnimatedActions))
            {
                if (!actions.Any(x => x.Name.Equals(animation.Key)))
                {
                    var action = new PlayerActionAnimation(animation.Key, animation.Value);
                    action.Initialize();
                    actions.Add(action);
                }
            }
            return actions;
        }

        public IEnumerable<PlayerActionAnimation> RegisterAnimations(IEnumerable<PlayerActionAnimation> loadedActions)
        {
            var actions = new List<PlayerActionAnimation>();
            foreach (var animation in loadedActions)
            {
                actions.Add(RegisterAnimation(animation));
            }
            return actions;
        }

        public PlayerActionAnimation RegisterAnimation(PlayerActionAnimation action)
        {
            action.Initialize();
            return action;
        }
    }
}