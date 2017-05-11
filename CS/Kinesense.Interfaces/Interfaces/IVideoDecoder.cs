using System;
using System.Net.Mime;
using System.Collections.Generic;

namespace Kinesense.Interfaces
{
	public interface IVideoDecoder : IDisposable
	{
		ContentType ContentType { get; }
		bool CanSeek { get; }
		DateTime? StartTime { get; }
		TimeSpan? Duration { get; }
		VideoFrame CurrentFrame { get; }
		bool Seek(DateTime requestTime);

		RecoveredFrame GetNextRecoveredFrame(int timeoutMilliseconds);

		VideoFrame GetNextFrame(int timeoutMilliseconds, ref bool exhaustedAllFrames);
		VideoFrame GetNextFrame(int timeoutMilliseconds);

		List<VideoFrame> GetAllFrames();

        byte[] GetMJpegBuffer();

		void StopProvidingFramesAndDisposeOfReader();

		event EventHandler VideoDecoderComplete;
	}

    
}
