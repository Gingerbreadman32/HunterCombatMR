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
        public CustomAction(string name,
                    string displayName = "")
            : base(name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Action name must not be blank!");

            DisplayName = string.IsNullOrEmpty(displayName) ? name : displayName;
            Animations = new ActionAnimations();
            Events = new ActionEvents<T>();
            FrameData = new SortedList<FrameIndex, KeyFrameData<ICustomAnimationV2>>();
        }

        public CustomAction(string name,
            ICustomAction<T> copy)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Action name must not be blank!");

            DisplayName = string.IsNullOrEmpty(displayName) ? name : displayName;
            Animations = new ActionAnimations();
            Events = new ActionEvents<T>(copy.Events);
            FrameData = copy.FrameData;
        }

        public ActionAnimations Animations { get; }
        public string DisplayName { get; }
        public ActionEvents<T> Events { get; }
        public SortedList<FrameIndex, KeyFrameData<ICustomAnimationV2>> FrameData { get; }

        public void ActionLogic(T entity,
            Animator animator)
        {
            foreach (var keyFrameEvent in Events.GetActiveTags(animator.CurrentKeyFrameIndex))
            {
                keyFrameEvent.InvokeLogic(entity, animator);
            }
        }

        public override IHunterCombatContentInstance CreateNew(string internalName)
        {
            throw new NotImplementedException();
        }
    }
}