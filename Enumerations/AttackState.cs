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
        AttackRecovery = 3,
        HitPause = 4,
        BlockStun = 5
    }
}