using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class CustomAction<TObject, TAnimationType> : HunterCombatContentInstance
        where TObject
        : IAnimated<TAnimationType> where TAnimationType
        : Animation
    {
        #region Public Properties

        public IEnumerable<TAnimationType> Animations { get; protected set; }

        public KeyFrameProfile FrameProfile { get; protected set; }
        public IEnumerable<KeyFrameEvent<TObject, TAnimationType>> KeyFrameEvents { get; protected set; }
        public string Name { get; protected set; }

        public IDictionary<string, string> ActionParameters { get; set; }

        #endregion Public Propertiesthods

        protected virtual void SetUpFrameProfile()
        {
            // There will always be at least two keyframes, start and end.
            int totalKeyFrames = 2;

            foreach (var animation in Animations)
            {
                totalKeyFrames++;
            }
        }

        protected void AddKeyFrameEvent(KeyFrame startKeyFrame,
            KeyFrame endKeyFrame,
            ActionLogicMethod<TObject, TAnimationType> actionLogicMethod)
        {
            var tempEvents = new List<KeyFrameEvent<TObject, TAnimationType>>(KeyFrameEvents);
            int newTag = 0;

            if (tempEvents.Any())
                newTag = tempEvents.Select(x => x.Tag).Max() + 1;

            tempEvents.Add(new KeyFrameEvent<TObject, TAnimationType>(newTag, startKeyFrame, endKeyFrame, actionLogicMethod));

            KeyFrameEvents = tempEvents;
        }

        protected void AddKeyFrameEvent(KeyFrame keyframe,
            ActionLogicMethod<TObject, TAnimationType> actionLogicMethod)
        {
            AddKeyFrameEvent(keyframe, keyframe, actionLogicMethod);
        }
    }
}