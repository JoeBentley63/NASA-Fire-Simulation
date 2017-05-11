using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    public enum TimelineCommand
    {
        Play,
        Pause,
        PlayPause,
        FastForward,
        PlayForwards,
        PlayBackwards,
        PreviousSprite,
        NextSprite,
        FastRewind,
        NextFrame,
        PreviousFrame,
        Null,
        PlayEventsOnly,
        RecordEventsOnly,
        Refresh,
        Record,
        SaveFrameToReport,
        OpenAviMaker,
        ScrollLeft,
        ScrollRight,
        ZoomIn,
        ZoomOut,
        ZoomToSelection,
        ZoomOutToMax,
        ChangePlayDirection,
        PlayFaster,
        PlaySlower,
        PlayNormal,
        Copy,
        LockorUnlockTimeline,
        ChangeTimelineMode,
        MuteAudio,
        AudioWave,
        WholeVideo,
        Other
    } ;
}
