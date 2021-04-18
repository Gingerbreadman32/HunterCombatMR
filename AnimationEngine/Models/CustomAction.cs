using HunterCombatMR.AnimationEngine.Interfaces;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace HunterCombatMR.AnimationEngine.Models
{
    public abstract class CustomAction<THolder, TEntity> : HunterCombatContentInstance,
        ICustomAction<THolder, TEntity> where THolder
        : IEntityHolder<TEntity> where TEntity
        : Entity
    {
        private IEnumerable<KeyFrameEvent<THolder, TEntity>> _internalEvents;
        private IEnumerable<EventTagInfo> _tagReferences;

        public CustomAction(string name,
                    string displayName = "")
            : base(name)
        {
            Name = (string.IsNullOrEmpty(displayName)) ? name : displayName;
            KeyFrameProfile = new KeyFrameProfile();
            DefaultParameters = new Dictionary<string, string>();
        }

        public IDictionary<string, string> ActionParameters { get; set; }
        public SortedList<int, IAnimation> Animations { get; protected set; }

        public IDictionary<string, string> DefaultParameters { get; set; }

        public IEnumerable<KeyFrameEvent<THolder, TEntity>> KeyFrameEvents
        {
            get => _internalEvents;
            set { _internalEvents = value; SetUpFrameProfile(); }
        }

        public KeyFrameProfile KeyFrameProfile { get; protected set; }
        public string Name { get; protected set; }

        public void AddKeyFrameEvent(ActionLogicMethod<THolder, TEntity> actionLogicMethod)
        {
            var tempEvents = new List<KeyFrameEvent<THolder, TEntity>>(KeyFrameEvents);
            int newTag = 0;

            if (tempEvents.Any())
                newTag = GetLowestFreeTag(tempEvents.Select(x => x.Tag));

            tempEvents.Add(new KeyFrameEvent<THolder, TEntity>(newTag, actionLogicMethod));

            KeyFrameEvents = tempEvents;
        }

        public IEnumerable<KeyFrameEvent<THolder, TEntity>> GetCurrentFrameEvents(int currentFrame)
        {
            var currentEvents = KeyFrameEvents.Where(x => _tagReferences.Any(y => y.TagReference.Equals(x.Tag)
                && y.IsActive(currentFrame)));

            if (currentEvents.Any())
                return new List<KeyFrameEvent<THolder, TEntity>>(currentEvents.OrderBy(x => x.Tag));

            return currentEvents;
        }

        protected virtual void SetUpFrameProfile()
        {
            // There will always be at least one keyframe.
            int totalKeyFrames = 1;
        }

        public void Initialize()
        {
            SetUpFrameProfile();
        }

        public virtual void Update(IAnimator animator)
        {
            foreach (var keyFrameEvent in GetCurrentFrameEvents(animator.CurrentFrame))
            {
                //animator.Parameters = keyFrameEvent.ActionLogic.ActionLogic(Player
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