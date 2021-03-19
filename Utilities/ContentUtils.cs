using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;

namespace HunterCombatMR.Utilities
{
    internal static class ContentUtils
    {
        #region Internal Methods

        internal static Animation GetAnim(string name)
            => HunterCombatMR.Instance.Content.GetContentInstance<Animation>(name);

        internal static T Get<T>(T content) where T : HunterCombatContentInstance
            => HunterCombatMR.Instance.Content.GetContentInstance<T>(content);

        internal static PlayerAction GetAttack(string name)
            => HunterCombatMR.Instance.Content.GetContentInstance<PlayerAction>(name);

        internal static MoveSet GetMoveSet(string name)
            => HunterCombatMR.Instance.Content.GetContentInstance<MoveSet>(name);

        #endregion Internal Methods
    }
}