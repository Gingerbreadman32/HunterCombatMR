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
        public CustomAction(string name,
            string displayName = "")
            : base(name)
        {
            Name = (string.IsNullOrEmpty(displayName)) ? name : displayName;
            FrameProfile = new KeyFrameProfile();
            ActionParameters = new Dictionary<string, string>();
        }

        public IEnumerable<TAnimationType> Animations { get; protected set; }

        public KeyFrameProfile FrameProfile { get; protected set; }
        public IEnumerable<KeyFrameEvent<TObject, TAnimationType>> KeyFrameEvents 
        { 
            get => _internalEvents; 
            set { _internalEvents = value; SetUpFrameProfile(); }
        }

        public string Name { get; protected set; }

        public IDictionary<string, string> ActionParameters { get; set; }

        private IEnumerable<KeyFrameEvent<TObject, TAnimationType>> _internalEvents;

        protected virtual void SetUpFrameProfile()
        {
            // There will always be at least one keyframe.
            int totalKeyFrames = 1;

            
        }

        public void AddKeyFrameEvent(KeyFrame keyFrame,
            ActionLogicMethod<TObject, TAnimationType> actionLogicMethod)
        {
            var tempEvents = new List<KeyFrameEvent<TObject, TAnimationType>>(KeyFrameEvents);
            int newTag = 0;

            if (tempEvents.Any())
                newTag = tempEvents.Select(x => x.Tag).Max() + 1;

            tempEvents.Add(new KeyFrameEvent<TObject, TAnimationType>(newTag, keyFrame, actionLogicMethod));

            KeyFrameEvents = tempEvents;
        }
    }
}