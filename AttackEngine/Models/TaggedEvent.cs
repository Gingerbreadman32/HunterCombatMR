namespace HunterCombatMR.AttackEngine.Models
{
    public class TaggedEvent<T>
    {
        public TaggedEvent(EventTag tag,
            Event<T> @event,
            bool startsEnabled = true)
        {
            Tag = tag;
            Event = @event;
            IsEnabled = startsEnabled;
        }

        public Event<T> Event { get; }
        public bool IsEnabled { get; set; } = true;
        public EventTag Tag { get; }
    }
}