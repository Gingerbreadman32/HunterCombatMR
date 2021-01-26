namespace HunterCombatMR.Enumerations
{
    /// <summary>
    /// Enumeration for the different states the player can be in
    /// </summary>
    public enum PlayerState
    {
        Neutral = 0,
        AttackStartup = 1,
        ActiveAttack = 2,
        AttackRecovery = 3,
        HitPause = 4,
        Jumping = 5,
        BlockStun = 6,
        Dead = 7
    }
}