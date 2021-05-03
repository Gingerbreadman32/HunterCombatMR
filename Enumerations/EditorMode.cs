using System.ComponentModel;

namespace HunterCombatMR.Enumerations
{
    public enum EditorMode
    {
        [Description("Play Mode")]
        None = 0,

        [Description("Action Edit")]
        ActionEdit = 1,

        [Description("Animation Edit")]
        AnimationEdit = 2
    }

    public static class EditorModePreset
    {
        public static EditorMode[] InEditor = new EditorMode[] { EditorMode.ActionEdit, EditorMode.AnimationEdit };
    }
}