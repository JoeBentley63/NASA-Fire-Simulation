using System;
using System.Net.Mime;

namespace Kinesense.Interfaces
{
	public interface IRecordedVideo
	{
		Guid ID { get; }
		DateTime StartTime { get; set; }
		DateTime EndTime { get; set; }
		ContentType ContentType { get; }
	}
}
