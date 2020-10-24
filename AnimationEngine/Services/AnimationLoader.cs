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

        public IEnumerable<LayeredAnimatedAction> RegisterAnimations()
        {
            var actions = new List<LayeredAnimatedAction>();
            foreach (var animation in Containers.SelectMany(x => x.AnimatedActions))
            {
                if (!actions.Any(x => x.Name.Equals(animation.Key)))
                {
                    var action = new LayeredAnimatedAction(animation.Key, animation.Value);
                    action.Initialize();
                    actions.Add(action);
                }
            }
            return actions;
        }
    }
}