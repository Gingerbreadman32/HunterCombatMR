﻿using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Extensions;
using HunterCombatMR.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
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

            Name = (string.IsNullOrEmpty(displayName)) ? name : displayName;
            KeyFrameProfile = new KeyFrameProfile();
            Animations = new ActionAnimationReference();
        }

        public ActionAnimationReference Animations { get; set; }

        public IEnumerable<TaggedEvent<T>> KeyFrameEvents
        {
            get => _events;
            set { _events = value; }
        }

        public KeyFrameProfile KeyFrameProfile { get; protected set; } // += operator
        public string Name { get; protected set; }

        public void AddKeyFrameEvent(Event<T> actionLogicMethod,
            FrameIndex startFrame)
        {
            var tempEvents = new List<TaggedEvent<T>>(KeyFrameEvents);
            int newTag = 0;

            if (tempEvents.Any())
                newTag = GetLowestFreeTag(tempEvents.Select(x => x.Tag.Id));

            tempEvents.Add(new TaggedEvent<T>(new EventTag(newTag, startFrame, startFrame + actionLogicMethod.LengthActive), actionLogicMethod));

            KeyFrameEvents = tempEvents;
        }

        public IEnumerable<TaggedEvent<T>> GetCurrentKeyFrameEvents(int currentKeyFrame)
        {
            var currentEvents = KeyFrameEvents.Where(x => x.IsEnabled && x.Tag.IsActive(currentKeyFrame));

            return currentEvents.OrderBy(x => x.Tag.Id).ToList();
        }

        public void Initialize<A>() where A : IAnimation
        {
            CreateKeyFrameProfile();
            Animations.LoadAnimations<A>();
        }

        public void ActionLogic(T entity,
            Animator animator)
        {
            foreach (var keyFrameEvent in GetCurrentKeyFrameEvents(animator.CurrentFrame))
            {
                keyFrameEvent.Event.InvokeLogic(entity, animator);
            }
        }

        private void CreateKeyFrameProfile()
        {
            int totalKeyFrames = 0;

            for (var i = 0; i < Animations.Count; i++)
            {
                if (!Animations.ContainsKey(i))
                    continue;

                var anim = Animations.GetAnimationReference(i).Item1;

                if (!anim.IsInitialized)
                    anim.Initialize();

                totalKeyFrames += anim.KeyFrameProfile.KeyFrameAmount; // Add all the keyframes using a += operator or something
                KeyFrameProfile.KeyFrameLengths.Add(KeyFrameProfile.KeyFrameLengths.Count(), anim.AnimationData.TotalFrames.ToFLength());
            }

            KeyFrameProfile.KeyFrameAmount = totalKeyFrames.ToFLength();

            
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