using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class CustomAction<TEntity, TAnimationType> : HunterCombatContentInstance,
        IAnimated, 
        ICustomAction<TEntity, TAnimationType> 
        where TEntity : IAnimatedEntity<TAnimationType> 
        where TAnimationType
        : Animation<TEntity, TAnimationType>
    {
        public CustomAction(string name,
            string displayName = "")
            : base(name)
        {
            Name = (string.IsNullOrEmpty(displayName)) ? name : displayName;
            KeyFrameProfile = new KeyFrameProfile();
            DefaultParameters = new Dictionary<string, string>();
        }

        public IEnumerable<TAnimationType> Animations { get; protected set; }

        public KeyFrameProfile KeyFrameProfile { get; protected set; }
        public IEnumerable<KeyFrameEvent<TEntity, TAnimationType>> KeyFrameEvents
        {
            get => _internalEvents;
            set { _internalEvents = value; SetUpFrameProfile(); }
        }

        public string Name { get; protected set; }

        public IDictionary<string, string> DefaultParameters { get; set; }

        private IEnumerable<KeyFrameEvent<TEntity, TAnimationType>> _internalEvents;

        protected virtual void SetUpFrameProfile()
        {
            // There will always be at least one keyframe.
            int totalKeyFrames = 1;


        }

        public void AddKeyFrameEvent(KeyFrame keyFrame,
            ActionLogicMethod<TEntity, TAnimationType> actionLogicMethod)
        {
            var tempEvents = new List<KeyFrameEvent<TEntity, TAnimationType>>(KeyFrameEvents);
            int newTag = 0;

            if (tempEvents.Any())
                newTag = tempEvents.Select(x => x.Tag).Max() + 1;

            tempEvents.Add(new KeyFrameEvent<TEntity, TAnimationType>(newTag, actionLogicMethod));

            KeyFrameEvents = tempEvents;
        }

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