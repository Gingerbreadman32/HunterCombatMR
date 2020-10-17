using HunterCombatMR.Attributes;

namespace HunterCombatMR.Enumerations
{
    /// <summary>
    /// Enumeration for storing the likely commands to use initiate combos
    /// </summary>
    public enum ComboInputs
    {
        [GameCommand("MouseLeft")]
        StandardAttack = 0,
        [GameCommand("MouseRight")]
        SpecialAttack = 1,
        [GameCommand("Jump")]
        Jump = 3,
        [GameCommand("Grapple")]
        Hook = 4,
        [GameCommand("Right")]
        Right = 5,
        [GameCommand("Left")]
        Left = 6,
        [GameCommand("Up")]
        Up = 7,
        [GameCommand("Down")]
        Down = 8
    }
}