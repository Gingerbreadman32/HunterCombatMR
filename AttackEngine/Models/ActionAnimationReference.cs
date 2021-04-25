using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.Extensions;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AttackEngine.Models
{
    public sealed class ActionAnimationReference
    {
        private SortedList<int, Tuple<IAnimation, FrameIndex>> _animations;

        internal ActionAnimationReference()
        {
            _animations = new SortedList<int, Tuple<IAnimation, FrameIndex>>();
            AnimationReferences = new Dictionary<int, string>();
        }

        public Type AnimationType { get; }
        public int Count { get => _animations.Count(); }
        internal IDictionary<int, string> AnimationReferences { get; set; }

        public void AddAnimation(IAnimation animation,
                    FrameIndex startKeyFrame)
        {
            _animations.Add(_animations.Count(), new Tuple<IAnimation, FrameIndex>(animation, startKeyFrame));
        }

        public void RemoveAnimation(int index)
        {
            if (!_animations.ContainsKey(index))
                return;

            DropDownAnimation(index);
        }

        public Tuple<IAnimation, FrameIndex> GetAnimationReference(int index)
            => _animations[index];

        public bool ContainsKey(int index)
            => _animations.ContainsKey(index);

        internal void LoadAnimations<T>() where T : IAnimation
        {
            _animations.Clear();
            foreach (var reference in AnimationReferences)
            {
                AddAnimation(ContentUtils.Get<T>(reference.Value), reference.Key.ToFIndex());
            }
        }

        internal IDictionary<int, string> SaveAnimations()
                    => _animations
                .Select(x => new KeyValuePair<int, string>(x.Value.Item2, x.Value.Item1.InternalName))
                .ToDictionary(x => x.Key, x => x.Value);

        private void DropDownAnimation(int index)
        {
            if (!_animations.ContainsKey(index + 1))
            {
                _animations.Remove(index);
                return;
            }

            _animations[index] = _animations[index + 1];
            DropDownAnimation(index + 1);
        }
    }
}