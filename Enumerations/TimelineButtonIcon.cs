using HunterCombatMR.Attributes;
using HunterCombatMR.UI;

namespace HunterCombatMR.Enumerations
{
    public enum TimelineButtonIcon
    {
        [TexturePath(UITexturePaths.TimelineTextures + "addicon")]
        Plus = 0,
        [TexturePath(UITexturePaths.TimelineTextures + "deleteicon")]
        Minus = 1,
        [TexturePath(UITexturePaths.TimelineTextures + "duplicateicon")]
        Duplicate = 2,
        [TexturePath(UITexturePaths.TimelineTextures + "moveicon")]
        Arrow = 3
    }
}