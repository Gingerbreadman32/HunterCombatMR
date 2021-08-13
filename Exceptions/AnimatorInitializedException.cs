using System;

namespace HunterCombatMR.Exceptions
{
    internal sealed class AnimatorInitializedException
        : Exception
    {
        private const string _message = "No animation set for this animator!";

        public AnimatorInitializedException()
            : base(_message)
        { }

        public AnimatorInitializedException(string message)
            : base(string.Format("{0} {1}", _message, message))
        { }
    }
}