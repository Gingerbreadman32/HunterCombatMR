namespace HunterCombatMR.Enumerations
{
    /// <summary>
    /// Enumeration for the different states the player can be in during an attack
    /// </summary>
    public enum PlayerAttackState
    {
        NotAttacking = 0,
        AttackStart = 1,
        Startup = 2,
        Active = 3,
        Recovery = 4,
        HitPause = 5
    }
}