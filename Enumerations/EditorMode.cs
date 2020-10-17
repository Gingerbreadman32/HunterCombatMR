using System.ComponentModel;

namespace HunterCombatMR.Enumerations
{
    public enum EditorMode
    {
        [Description("Play Mode")]
        None = 0,
        [Description("View Mode")]
        ViewMode = 1,
        [Description("Edit Mode")]
        EditMode = 2
    }
}