using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kinesense.Interfaces
{
	public abstract class AbstractDatabaseVideoExporter
	{
		public virtual BinaryWriter Writer { get; set; }
		public int CountExportFrames { get; protected set; }
		public DateTime? FirstExportedFrameTime { get; protected set; }
		public DateTime? LastExportedFrameTime { get; protected set; }
		public TimeSpan CurrentExportVideoLength { get; protected set; }
		public TimeSpan TotalExportVideoLength { get; protected set; }
		public TimeSpan SumClipsDuration { get; protected set; }
		public DateTime? NextVideoSegmentTime { get; protected set; }

		public virtual void Reset()
		{
			CountExportFrames = 0;
			FirstExportedFrameTime = null;
			LastExportedFrameTime = null;
			CurrentExportVideoLength = TimeSpan.Zero;
			TotalExportVideoLength = TimeSpan.Zero;
			SumClipsDuration = TimeSpan.Zero;
			NextVideoSegmentTime = null;
		}

		/// <summary>
		/// looks at each frame and processes it. Slow
		/// </summary>
		/// <param name="mode"></param>
		public abstract void CopyNextVideoFramePacketFromDatabaseAndDeinterlace(DeinterlaceMode mode, VideoClipInfo clip);

		/// <summary>
		/// straight copy from the database - fast.
		/// </summary>
		public abstract void CopyNextVideoFramePacketFromDatabase(VideoClipInfo clip);

		#region IDisposable Members

		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		#endregion

	}
}
