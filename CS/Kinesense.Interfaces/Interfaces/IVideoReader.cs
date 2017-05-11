using System;
using System.IO;
using System.Net.Mime;

namespace Kinesense.Interfaces
{
	public interface IVideoReader : IDisposable
	{
		ContentType ContentType { get; }
		DateTime? ContentCreationTime { get; }
		Stream BaseStream { get; }
		String ContentTypeSubType { get; }

        string FilePath { get; }
    }

	public interface IVideoReader<TSource> : IVideoReader
	{
		TSource Source { get; }
	}
}