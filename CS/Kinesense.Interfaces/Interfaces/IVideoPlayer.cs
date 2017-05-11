using Kinesense.Interfaces.ImportExport;
using System;
using System.Collections.Generic;

namespace Kinesense.Interfaces
{
	public interface IVideoPlayer : IDisposable
	{
		void Play();
		void Pause();
		void Stop();

		bool CanSeek { get; }
		DateTime? StartTime { get; }
		TimeSpan? Duration { get; }
		VideoFrame CurrentFrame { get; }
		bool Seek(DateTime requestTime);

		VideoFrame GetNextFrame(int timeoutMilliseconds);
        RecoveredFrame GetNextRecoveredFrame(int timeoutMilliseconds);

        Dictionary<int, IVideoRecorder> ChannelRecorderDictionary { get; set; }

        IVideoRecorder VideoRecorder { get; set; }
        bool CanUseVideoRecorder { get; }
    }

	public interface IVideoPlayer<TSource> : IVideoPlayer
	{
		TSource Source { get; }
	}
}