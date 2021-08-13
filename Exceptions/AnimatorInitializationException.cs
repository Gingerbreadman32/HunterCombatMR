using System;

namespace HunterCombatMR.Exceptions
{
    internal sealed class AnimatorInitializationException
        : Exception
    {
        private const string _message = "Animator failed to initialize animation!";

        public AnimatorInitializationException()
            : base(_message)
        { }

        public AnimatorInitializationException(string message)
            : base(string.Format("{0} {1}", _message, message))
        { }

        public AnimatorInitializationException(string message, Exception innerException)
            : base(string.Format("{0} {1}", _message, message), innerException)
        { }
    }
}