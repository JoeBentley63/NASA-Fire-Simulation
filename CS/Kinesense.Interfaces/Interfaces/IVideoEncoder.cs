using System;
using System.Net.Mime;
using System.Collections.Generic;
using Kinesense.Interfaces.Bitmaps;

namespace Kinesense.Interfaces
{
	public interface IVideoEncoder : IDisposable
	{
		IVideoWriter Writer { get; }
		bool IsKeyFrame(VideoFrame frame);
        void AddFrame(IVideoFrame frame);
        void AddFrames(IList<VideoFrame> frames);

		ContentType ContentType { get; }
	}
}
