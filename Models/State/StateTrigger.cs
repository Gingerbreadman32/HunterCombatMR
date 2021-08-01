using System;

namespace HunterCombatMR.Models.State
{
    public struct StateTrigger
    {
        public StateTrigger(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new Exception("Trigger must have some text to convert!");

            AsText = text;
            Logic = Convert(AsText);
        }

        public string AsText { get; }
        public Func<int, int, bool> Logic { get; }

        private static Func<int, int, bool> Convert(string text)
        {
            // Fill this
            return (x, y) => { return false; };
        }

        private string Convert(Func<int, int, bool> logic)
        {
            // Fill this
            return "";
        }
    }
}