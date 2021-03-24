using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AttackEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class CustomAction<TEntity, TActionType> : HunterCombatContentInstance,
        IAnimated, 
        ICustomAction<TEntity, TActionType> 
        where TEntity : IAnimatedEntity<TActionType> 
        where TActionType
        : Animation<TEntity, TActionType>
    {
        #region Private Fields

        private IEnumerable<KeyFrameEvent<TEntity, TActionType>> _internalEvents;
        private IEnumerable<EventTag> _tagReferences;

        #endregion Private Fields

        #region Public Constructors

        public CustomAction(string name,
                    string displayName = "")
            : base(name)
        {
            Name = (string.IsNullOrEmpty(displayName)) ? name : displayName;
            KeyFrameProfile = new KeyFrameProfile();
            DefaultParameters = new Dictionary<string, string>();
        }

        #endregion Public Constructors

        #region Public Properties

        public IDictionary<string, string> ActionParameters { get; set; }
        public IEnumerable<TActionType> Animations { get; protected set; }

        public KeyFrameProfile KeyFrameProfile { get; protected set; }
        public IEnumerable<KeyFrameEvent<TEntity, TActionType>> KeyFrameEvents
        {
            get => _internalEvents;
            set { _internalEvents = value; SetUpFrameProfile(); }
        }

        public string Name { get; protected set; }

        public IDictionary<string, string> DefaultParameters { get; set; }

       

        public void AddKeyFrameEvent(ActionLogicMethod<TEntity, TActionType> actionLogicMethod)
        {
            var tempEvents = new List<KeyFrameEvent<TEntity, TActionType>>(KeyFrameEvents);
            int newTag = 0;

            if (tempEvents.Any())
                newTag = tempEvents.Select(x => x.Tag).Max() + 1;

            tempEvents.Add(new KeyFrameEvent<TEntity, TActionType>(newTag, actionLogicMethod));

            KeyFrameEvents = tempEvents;
        }

        public IEnumerable<KeyFrameEvent<TEntity, TActionType>> GetCurrentFrameEvents(int currentFrame)
        {
            var currentEvents = KeyFrameEvents.Where(x => _tagReferences.Any(y => y.TagReference.Equals(x.Tag)
                && y.CheckFrameBetween(currentFrame)));

            if (currentEvents.Any())
                return new List<KeyFrameEvent<TEntity, TActionType>>(currentEvents.OrderBy(x => x.Tag));

            return currentEvents;
        }
        
        #endregion Public Methods

        #region Protected Methods

        protected virtual void SetUpFrameProfile()
        {
            // There will always be at least one keyframe.
            int totalKeyFrames = 1;
        }

        #endregion Protected Methods
        public void Initialize()
        {
            SetUpFrameProfile();
        }

        public virtual void Update(IAnimator animator)
        {
            /* foreach (var keyFrameEvent in KeyFrameEvents.Where(x => (x.KeyFrame.IsKeyFrameActive(currentFrame)
         || x.EndKeyFrame.IsKeyFrameActive(currentFrame))
         && x.IsEnabled).OrderBy(x => x.Tag))
            {
                animator.Parameters = keyFrameEvent.ActionLogic.ActionLogic(Player,
                    ,
                    (currentFrame - currentKeyFrame.StartingFrameIndex),
                    animator.Parameters); */
        }
    }
}
