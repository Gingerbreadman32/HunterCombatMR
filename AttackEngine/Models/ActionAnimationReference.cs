using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AnimationEngine.Models;
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
        public int Count => _animations.Count();
        internal IDictionary<int, string> AnimationReferences { get; set; }

        public void AddAnimation(IAnimation animation,
            FrameIndex keyFrameIndex)
        {
            _animations.Add(_animations.Count(), new Tuple<IAnimation, FrameIndex>(animation, keyFrameIndex));
        }

        public bool ContainsKey(int index)
            => _animations.ContainsKey(index);

        public IAnimation GetAnimationByKeyFrame(FrameIndex keyFrame)
        {
            var firstAnim = _animations.First(x => x.Value.Item2 <= keyFrame
                && x.Value.Item2 + x.Value.Item1.KeyFrameProfile.KeyFrameAmount >= keyFrame);

            return firstAnim.Value.Item1;
        }

        public Tuple<IAnimation, FrameIndex> GetAnimationReference(int index)
            => _animations[index];

        public void RemoveAnimation(int index)
        {
            if (!_animations.ContainsKey(index))
                return;

            DropDownAnimation(index);
        }

        internal void LoadAnimations<T>() where T : IAnimation
        {
            _animations.Clear();
            FrameIndex totalKeyFrames = FrameIndex.Zero;
            foreach (var reference in AnimationReferences.OrderBy(x => x.Key))
            {
                var animation = ContentUtils.Get<T>(reference.Value);
                AddAnimation(animation, totalKeyFrames);
                totalKeyFrames += animation.KeyFrameProfile.KeyFrameAmount;
            }
        }

        internal IDictionary<int, string> SaveAnimations()
                    => _animations
                .Select(x => new KeyValuePair<int, string>(x.Key, x.Value.Item1.InternalName))
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