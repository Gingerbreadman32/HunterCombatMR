using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Interfaces;

namespace HunterCombatMR.Utilities
{
    internal static class ContentUtils
    {
        #region Internal Methods

        internal static PlayerAnimation GetPlayerAnim(string name)
            => HunterCombatMR.Instance.Content.GetContentInstance<PlayerAnimation>(name);

        internal static T Get<T>(T content) where T : IHunterCombatContentInstance
            => HunterCombatMR.Instance.Content.GetContentInstance<T>(content);

        internal static PlayerAction GetPlayerAction(string name)
            => HunterCombatMR.Instance.Content.GetContentInstance<PlayerAction>(name);

        internal static MoveSet GetMoveSet(string name)
            => HunterCombatMR.Instance.Content.GetContentInstance<MoveSet>(name);

        #endregion Internal Methods
    }
}