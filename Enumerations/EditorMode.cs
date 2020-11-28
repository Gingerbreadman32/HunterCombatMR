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

    public static class EditorModePreset
    {
        public static EditorMode[] InEditor = new EditorMode[] { EditorMode.ViewMode, EditorMode.EditMode };
        public static EditorMode[] ViewOnly = new EditorMode[] { EditorMode.ViewMode };
        public static EditorMode[] ViewOrIngame = new EditorMode[] { EditorMode.ViewMode, EditorMode.None };
        public static EditorMode[] EditOnly = new EditorMode[] { EditorMode.EditMode };
    }
}