namespace HunterCombatMR.Enumerations
{
    /// <summary>
    /// The current attack state of the entity
    /// </summary>
    public enum AttackState
    {
        NotAttacking = 0,
        AttackStartup = 1,
        ActiveAttack = 2,
        AttackCancellable = 3,
        AttackRecovery = 4,
        HitPause = 5,
        BlockStun = 6
    }
}