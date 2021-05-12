using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models
{
    public abstract class CustomAction<T> : HunterCombatContentInstance,
        IAnimated,
        INamed
    {
        private IEnumerable<TaggedEvent<T>> _events;

        public CustomAction(string name,
                    string displayName = "")
            : base(name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Action name must not be blank!");

            Name = string.IsNullOrEmpty(displayName) ? name : displayName;
            KeyFrameProfile = new KeyFrameProfile();
            Animations = new ActionAnimationReference();
            _events = new List<TaggedEvent<T>>();
        }

        public ActionAnimationReference Animations { get; protected set; }

        public IEnumerable<TaggedEvent<T>> KeyFrameEvents
        {
            get => _events;
            protected set { _events = value; }
        }

        public KeyFrameProfile KeyFrameProfile { get; protected set; }

        public string Name { get; protected set; }

        public void ActionLogic(T entity,
            Animator animator)
        {
            foreach (var keyFrameEvent in GetCurrentKeyFrameEvents(animator.CurrentKeyFrameIndex))
            {
                keyFrameEvent.Event.InvokeLogic(entity, animator);
            }
        }

        public void AddKeyFrameEvent(Event<T> actionLogicMethod,
            FrameIndex startFrame)
        {
            var tempEvents = new List<TaggedEvent<T>>(KeyFrameEvents);
            int newTag = 0;

            if (tempEvents.Any())
                newTag = GetLowestFreeTag(tempEvents.Select(x => x.Tag.Id));

            tempEvents.Add(new TaggedEvent<T>(new EventTag(newTag, startFrame, startFrame + ((int)actionLogicMethod.LengthActive - 1)), actionLogicMethod));

            KeyFrameEvents = tempEvents;
        }

        public IEnumerable<TaggedEvent<T>> GetCurrentKeyFrameEvents(int currentKeyFrame)
        {
            var currentEvents = KeyFrameEvents.Where(x => x.IsEnabled && x.Tag.IsActive(currentKeyFrame));

            return currentEvents.OrderBy(x => x.Tag.Id).ToList();
        }

        public void Initialize<A>() where A : ICustomAnimation
        {
            Animations.LoadAnimations<A>();
            CreateKeyFrameProfile();
        }

        private void CreateKeyFrameProfile()
        {
            KeyFrameProfile.Clear();
            for (var i = 0; i < Animations.Count; i++)
            {
                if (!Animations.ContainsKey(i))
                    continue;

                var anim = Animations.GetAnimationReference(i).Item1;

                if (!anim.IsInitialized)
                    anim.Initialize();

                if (i == 0)
                {
                    KeyFrameProfile = new KeyFrameProfile(anim.KeyFrameProfile);
                    continue;
                }

                KeyFrameProfile += anim.KeyFrameProfile;
            }
        }

        private int GetLowestFreeTag(IEnumerable<int> tags)
        {
            bool[] checkedTags = new bool[tags.Count() + 1];

            foreach (int tag in tags)
                if (tag <= tags.Count())
                    checkedTags[tag] = true;

            for (int t = 0; t < tags.Count(); t++)
                if (!checkedTags[t])
                    return t;

            return tags.Count();
        }
    }
}