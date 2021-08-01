namespace HunterCombatMR.Enumerations
{
    /// <summary>
    /// Enumeration for the different basic positions an entity can be in
    /// </summary>
    public enum EntityWorldStatus
    {
        NoStatus = 0,
        Grounded = 1,
        Walking = 2,
        Aerial = 3,
        Jumping = 4,
        Dead = 5
    }
}