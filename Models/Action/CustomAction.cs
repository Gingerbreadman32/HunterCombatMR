using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Action;
using System;

namespace HunterCombatMR.Models.Action
{
    public class CustomAction<T>
        : Content,
        IDisplayNamed,
        ICustomAction<T>
    {
        public CustomAction(string name,
            string displayName = "")
            : base(name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Action name must not be blank!");

            DisplayName = string.IsNullOrEmpty(displayName) ? name : displayName;
            Animations = new ActionAnimations();
            Events = new ActionEvents<T>();
        }

        public CustomAction(string name,
            string animation,
            string displayName = "")
            : base(name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Action name must not be blank!");

            DisplayName = string.IsNullOrEmpty(displayName) ? name : displayName;
            Animations = new ActionAnimations(animation);
            Events = new ActionEvents<T>();
        }

        public CustomAction(string name,
            ICustomAction<T> copy)
            : base(copy.InternalName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Action name must not be blank!");

            DisplayName = copy.DisplayName;
            Animations = new ActionAnimations(copy.Animations);
            Events = new ActionEvents<T>(copy.Events);
        }

        public ActionAnimations Animations { get; }
        public string DisplayName { get; set; }
        public ActionEvents<T> Events { get; }

        public void ActionLogic(T entity,
            Animator animator)
        {
            foreach (var keyFrameEvent in Events.GetActiveTags(animator.CurrentKeyFrameIndex))
            {
                keyFrameEvent.InvokeLogic(entity, animator);
            }
        }

        public override IContent CreateNew(string internalName)
            => new CustomAction<T>(internalName, this);

        // Create frame data from ordered animations
    }
}