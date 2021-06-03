using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Action;
using HunterCombatMR.Interfaces.Animation;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.Models.Action
{
    public class CustomAction<T>
        : HunterCombatContentInstance,
        IDisplayNamed,
        ICustomAction<T>
    {
        private readonly SortedList<FrameIndex, KeyFrameData<ActionAnimationKeyframe>> _frameData;

        public CustomAction(string name,
            string displayName = "")
            : base(name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Action name must not be blank!");

            DisplayName = string.IsNullOrEmpty(displayName) ? name : displayName;
            Animations = new ActionAnimations();
            Events = new ActionEvents<T>();
            _frameData = new SortedList<FrameIndex, KeyFrameData<ActionAnimationKeyframe>>();
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
            _frameData = new SortedList<FrameIndex, KeyFrameData<ActionAnimationKeyframe>>(copy.FrameData);
        }

        public ActionAnimations Animations { get; }
        public string DisplayName { get; set; }
        public ActionEvents<T> Events { get; }
        public SortedList<FrameIndex, KeyFrameData<ActionAnimationKeyframe>> FrameData => _frameData;

        public void ActionLogic(T entity,
            Animator animator)
        {
            foreach (var keyFrameEvent in Events.GetActiveTags(animator.CurrentKeyFrameIndex))
            {
                keyFrameEvent.InvokeLogic(entity, animator);
            }
        }

        public override IHunterCombatContentInstance CreateNew(string internalName)
            => new CustomAction<T>(internalName, this);

        // Create frame data from ordered animations
    }
}