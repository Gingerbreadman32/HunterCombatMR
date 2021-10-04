namespace HunterCombatMR.Services
{
    public abstract class ManagerBase
    {
        private bool _initialized;
        public bool Initialized { get => _initialized; }

        public ManagerBase() { }

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

        protected virtual void OnDispose() { }

        protected virtual void OnInitialize() { }
    }
}