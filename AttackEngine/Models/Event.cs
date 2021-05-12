using HunterCombatMR.Interfaces;
using HunterCombatMR.Models;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AttackEngine.Models
{
    public abstract class Event<T>
        : INamed
    {
        public Event()
        {
            Name = GetType().Name;
            CurrentParameters = (DefaultParameters != null)
                ? DefaultParameters.ToList()
                : new List<EventParameter>();

            if (GetParameter("Active") == null)
                CurrentParameters.Add(new EventParameter("Active", 1));
        }

        public Event(FrameLength length)
        {
            Name = GetType().Name;
            LengthActive = length;

            CurrentParameters = (DefaultParameters != null)
                ? DefaultParameters.ToList()
                : new List<EventParameter>();

            if (GetParameter("Active") == null)
                CurrentParameters.Add(new EventParameter("Active", 1));
        }

        public IList<EventParameter> CurrentParameters { get; private set; }

        public virtual IEnumerable<EventParameter> DefaultParameters { get; }

        public FrameLength LengthActive { get; } = FrameLength.One;

        public string Name { get; }

        public float GetParameterValue(string name)
            => GetParameter(name).Value;

        public abstract void InvokeLogic(T entity,
                    Animator animator);

        public virtual bool IsActive()
        {
            if (GetParameter("Active") != null)
                return GetParameter("Active").Value.Equals(1);

            return false;
        }

        public void MakeParameterDefault(string name)
            => ModifyParameter(name, GetParameter(name).DefaultValue);

        public void ModifyParameter(string name,
            float newValue)
            => GetParameter(name).Value = newValue;

        private EventParameter GetParameter(string name)
                                                    => CurrentParameters.SingleOrDefault(x => x.Name.Equals(name));
    }
}