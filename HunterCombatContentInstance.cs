using HunterCombatMR.Interfaces;
using Newtonsoft.Json;
using System;

namespace HunterCombatMR
{
    public abstract class HunterCombatContentInstance 
        : IHunterCombatContentInstance
    {
        public HunterCombatContentInstance(string name)
        {
            InternalName = name;
        }

        #region Public Properties

        [JsonIgnore]
        public virtual string InternalName { get; }

        #endregion Public Properties

        #region Public Methods

        public virtual T Duplicate<T>(string name) where T : IHunterCombatContentInstance
            => (T)typeof(T).GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { name });

        #endregion Public Methods
    }
}