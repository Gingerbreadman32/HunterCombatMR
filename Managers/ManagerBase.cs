namespace HunterCombatMR.Services
{
    public abstract class ManagerBase
    {
        private static bool _initialized;
        public static bool Initialized { get => _initialized; }

        public ManagerBase()
        {

        }

        public void Dispose()
        {
            OnDispose();
            _initialized = false;
        }

        public void Initialize()
        {
            OnInitialize();
            _initialized = true;
        }

        protected abstract void OnDispose();

        protected abstract void OnInitialize();
    }
}