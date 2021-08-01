namespace HunterCombatMR.Enumerations
{
    /// <summary>
    /// The current action state of the entity
    /// </summary>
    public enum EntityActionStatus
    {
        Idle = 0,
        ActionStartup = 1,
        CurrentlyActing = 2,
        CancelableRecovery = 3,
        ForcedRecovery = 4,
        HitStun = 5,
        BlockStun = 6
    }
}