using HunterCombatMR.Attributes;
using HunterCombatMR.Constants;

namespace HunterCombatMR.Enumerations
{
    public enum TimelineButtonIcon
    {
        [TexturePath(UITexturePaths.TimelineTextures + "xicon")]
        NotFound = 0,
        [TexturePath(UITexturePaths.TimelineTextures + "deleteicon")]
        Minus = 1,
        [TexturePath(UITexturePaths.TimelineTextures + "duplicateicon")]
        Duplicate = 2,
        [TexturePath(UITexturePaths.TimelineTextures + "moveicon")]
        LeftArrow = 3,
        [TexturePath(UITexturePaths.TimelineTextures + "moveicon")]
        RightArrow = 4,
        [TexturePath(UITexturePaths.TimelineTextures + "playicon")]
        Play = 5,
        [TexturePath(UITexturePaths.TimelineTextures + "pauseicon")]
        Pause = 6,
        [TexturePath(UITexturePaths.TimelineTextures + "stopicon")]
        Stop = 7,
        [TexturePath(UITexturePaths.TimelineTextures + "loopicon")]
        Loop = 8,
        [TexturePath(UITexturePaths.TimelineTextures + "playpauseloopicon")]
        PlayPause = 9,
        [TexturePath(UITexturePaths.TimelineTextures + "looponceicon")]
        LoopOnce = 10,
        [TexturePath(UITexturePaths.TimelineTextures + "pingpongicon")]
        PingPong = 11,
        [TexturePath(UITexturePaths.TimelineTextures + "addicon")]
        Plus = 12
    }
}