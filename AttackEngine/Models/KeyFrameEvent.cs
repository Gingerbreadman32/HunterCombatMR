using HunterCombatMR.Interfaces;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public class KeyFrameEvent<THolder, TEntity> where THolder
        : IEntityHolder<TEntity> where TEntity
        : Entity
    {
        public KeyFrameEvent(int tag,
            ActionLogicMethod<THolder, TEntity> actionLogic)
        {
            Tag = tag;
            ActionLogic = actionLogic;
        }

        public ActionLogicMethod<THolder, TEntity> ActionLogic { get; }
        public bool IsEnabled { get; set; } = true;
        public int Tag { get; }
    }
}