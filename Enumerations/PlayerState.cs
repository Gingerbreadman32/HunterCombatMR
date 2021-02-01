namespace HunterCombatMR.Enumerations
{
    /// <summary>
    /// Enumeration for the different states the player can be in
    /// </summary>
    public enum PlayerState
    {
        Neutral = 0,
        Walking = 1,
        Aerial = 2,
        AttackStartup = 3,
        ActiveAttack = 4,
        AttackRecovery = 5,
        HitPause = 6,
        Jumping = 7,
        BlockStun = 8,
        Dead = 9
    }
}