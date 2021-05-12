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
            IEnumerable<Tuple<EventTag, bool, string>> events)
            : base(name, displayName)
        {
            Animations.AnimationReferences = animations;

            var setEvents = new List<TaggedEvent<HunterCombatPlayer>>();

            foreach (var taggedEvent in events)
            {
                setEvents.Add(new TaggedEvent<HunterCombatPlayer>(taggedEvent.Item1,
                    HunterCombatMR.Instance.GetPlayerActionEvent(taggedEvent.Item3),
                    taggedEvent.Item2));
            }

            KeyFrameEvents = setEvents;
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

            Initialize<PlayerAnimation>();
        }

        public override IHunterCombatContentInstance CloneFrom(string internalName)
            => new PlayerAction(this, internalName);
    }
}