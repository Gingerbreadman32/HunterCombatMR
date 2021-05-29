using HunterCombatMR.Interfaces;
using HunterCombatMR.Interfaces.Animation;
using HunterCombatMR.Models;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Action
{
    public sealed class ActionAnimations
        : ICompact<string>
    {
        private SortedList<int, ICustomAnimationV2> _animations;

        public ActionAnimations()
        {
            _animations = new SortedList<int, ICustomAnimationV2>();
        }

        public ActionAnimations(string[] animations)
        {
            _animations = new SortedList<int, ICustomAnimationV2>();

            for (var a = 0; a < animations.Count(); a++)
            {
                AddAnimation(animations[a]);
            }
        }

        public int Count => _animations.Count();

        public void AddAnimation(ICustomAnimationV2 animation)
        {
            _animations.Add(Count, animation);
        }

        public void AddAnimation(string animationName)
        {
            ICustomAnimationV2 animation;

            if (!ContentUtils.TryGet(animationName, out animation))
                throw new ArgumentOutOfRangeException($"Animation with name {animationName} does not exist within loaded animations!");

            AddAnimation(animation);
        }

        /*
        public KeyFrameData CreateActionFrameData()
        {
            if (!_animations.Any())
                throw new KeyNotFoundException("No animations initalized for this action!");

            KeyFrameData temp = _animations[0].FrameData;

            for (var i = 1; i < _animations.Count; i++)
            {
                if (!_animations.ContainsKey(i))
                    continue;

                var data = _animations[i].FrameData;

                temp += data;
            }

            return temp;
        }
        */

        public FrameIndex MoveAnimation(FrameIndex currentKeyframe,
                    FrameIndex newKeyframe)
        {
            if (!_animations.ContainsKey(currentKeyframe))
                throw new ArgumentOutOfRangeException($"No animation exists at this keyframe: {currentKeyframe}!");

            var reference = _animations[currentKeyframe];

            MigrateKeyframe(reference, newKeyframe);

            return _animations.IndexOfValue(reference);
        }

        public void RemoveAnimation(FrameIndex index)
        {
            if (!_animations.ContainsKey(index))
                return;

            DropDownAnimation(index);
        }

        public string[] Save()
                    => _animations.Select(x => x.Value.InternalName).ToArray();

        public void SwitchAnimations(FrameIndex firstAnimationIndex,
                    FrameIndex secondAnimationIndex)
        {
            if (!_animations.ContainsKey(firstAnimationIndex))
                throw new ArgumentOutOfRangeException($"No animation exists at this keyframe: {firstAnimationIndex}!");

            if (!_animations.ContainsKey(secondAnimationIndex))
                throw new ArgumentOutOfRangeException($"No animation exists at this keyframe: {secondAnimationIndex}!");

            var left = _animations[firstAnimationIndex];
            var right = _animations[secondAnimationIndex];

            _animations[secondAnimationIndex] = left;
            _animations[firstAnimationIndex] = right;
        }

        /// <summary>
        /// Tries and gets the animation that starts at the specified keyframe.
        /// </summary>
        /// <param name="keyframe">The keyframe or order that the animation will start</param>
        /// <param name="animation">The animation, null if returns false</param>
        /// <returns>True if there is an animation that starts on the keyframe, false if not.</returns>
        public bool TryGetAnimation(FrameIndex keyframe, out ICustomAnimationV2 animation)
        {
            if (!_animations.ContainsKey(keyframe))
            {
                animation = null;
                return false;
            }

            animation = _animations[keyframe];
            return animation != null;
        }

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

        private void MigrateKeyframe(ICustomAnimationV2 reference,
            FrameIndex targetPosition)
        {
            bool placeEarlier = _animations.IndexOfValue(reference) > targetPosition;

            if (targetPosition >= Count)
                targetPosition = Count - 1;

            while (_animations.IndexOfValue(reference) != targetPosition)
            {
                int modifier = placeEarlier ? -1 : 1;

                if (!_animations.ContainsKey(_animations.IndexOfValue(reference) + modifier))
                {
                    return;
                }
                SwitchAnimations(_animations.IndexOfValue(reference), _animations.IndexOfValue(reference) + modifier);
            }
        }
    }
}