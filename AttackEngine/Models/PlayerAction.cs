using HunterCombatMR.Events;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Models;
using System;
using System.Collections.Generic;

namespace HunterCombatMR.AttackEngine.Models
{
    public class PlayerAction
        : CustomAction<HunterCombatPlayer>
    {
        public PlayerAction(string name,
            string displayName = "")
            : base(name, displayName)
        {
        }

        public PlayerAction(string name,
            string displayName,
            IDictionary<int, string> animations,
            IEnumerable<Tuple<EventTag, bool, string>> keyFrameEvents,
            IDictionary<string, bool> lifetimeEvents)
            : base(name, displayName)
        {
            Animations.AnimationReferences = animations;

            var setEvents = new List<TaggedEvent<HunterCombatPlayer>>();
            var lifeEvents = new Dictionary<Event<HunterCombatPlayer>, bool>();

            foreach (var taggedEvent in keyFrameEvents)
            {
                setEvents.Add(new TaggedEvent<HunterCombatPlayer>(taggedEvent.Item1,
                    HunterCombatMR.Instance.GetPlayerActionEvent(taggedEvent.Item3),
                    taggedEvent.Item2));
            }

            foreach (var lifetimeEvent in lifetimeEvents)
            {
                lifeEvents.Add(HunterCombatMR.Instance.GetPlayerActionEvent(lifetimeEvent.Key), lifetimeEvent.Value);
            }

            KeyFrameEvents = setEvents;
            LifetimeEvents = lifeEvents;
            Initialize<PlayerAnimation>();
        }

        public PlayerAction(PlayerAction copy,
            string name)
            : base(name, copy.Name)
        {
            Animations.AnimationReferences = copy.Animations.AnimationReferences;
            KeyFrameProfile = new KeyFrameProfile(copy.KeyFrameProfile);
            KeyFrameEvents = new List<TaggedEvent<HunterCombatPlayer>>(copy.KeyFrameEvents);
            IsStoredInternally = copy.IsStoredInternally;
            LifetimeEvents = new Dictionary<Event<HunterCombatPlayer>, bool>(copy.LifetimeEvents);

            Initialize<PlayerAnimation>();
        }

        public override IHunterCombatContentInstance CloneFrom(string internalName)
            => new PlayerAction(this, internalName);
    }
}