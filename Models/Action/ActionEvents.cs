using HunterCombatMR.AttackEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.Action
{
    public class ActionEvents<T>
    {
        private IEnumerable<TaggedEvent<T>> _events;

        private IEnumerable<string> _eventTriggers;

        public ActionEvents()
        {
            _events = new List<TaggedEvent<T>>();
        }

        public ActionEvents(ActionEvents<T> copy)
        {
            _events = new List<TaggedEvent<T>>();

            foreach (var actionEvent in copy._events)
            {
                AddKeyframeEvent(actionEvent.Event, actionEvent.Tag.StartKeyframe);
            }
        }

        public void AddKeyframeEvent(Event<T> keyframeEvent,
           FrameIndex keyframe)
        {
            AddEvent(keyframeEvent,
                new EventTag(GetLowestFreeTag(), keyframe, keyframeEvent.LengthActive));
        }

        public void AddLifetimeEvent(Event<T> lifetimeEvent)
        {
            AddEvent(lifetimeEvent,
                new EventTag(GetLowestFreeTag()));
        }

        public IEnumerable<Event<T>> GetActiveTags(FrameIndex keyframe)
        {
            return _events.Where(x => x.IsEnabled && x.Tag.IsActive(keyframe)).Select(x => x.Event);
        }

        private void AddEvent(Event<T> @event, EventTag tag)
        {
            var tempEvents = new List<TaggedEvent<T>>(_events);

            tempEvents.Add(new TaggedEvent<T>(tag, @event));

            _events = tempEvents;
        }

        private int GetLowestFreeTag()
        {
            IEnumerable<int> tags = _events.Select(x => x.Tag.ID);
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