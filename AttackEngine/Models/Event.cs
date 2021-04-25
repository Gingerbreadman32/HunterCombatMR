using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Interfaces;
using System.Linq;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class Event<T>
        : INamed
    {
        public EventParameter[] CurrentParameters { get; private set; }
        public FrameLength LengthActive { get; } = FrameLength.One;

        public virtual string Name { get => GetType().Name; }

        public EventParameter GetParameter(string name)
            => CurrentParameters.Single(x => x.Name.Equals(name));

        public abstract void InvokeLogic(T entity,
                    Animator animator);

        public void ModifyParameter(string name,
            float newValue)
            => CurrentParameters.Single(x => x.Name.Equals(name)).Value = newValue;
    }
}