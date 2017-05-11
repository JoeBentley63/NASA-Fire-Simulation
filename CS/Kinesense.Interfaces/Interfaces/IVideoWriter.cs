using System;
using System.IO;

namespace Kinesense.Interfaces
{
	public interface IVideoWriter : IDisposable
	{
        string FilePath { get; }
		object Destination { get; }
		Stream BaseStream { get; }

    }

	public interface IVideoWriter<T> : IVideoWriter
	{
		new T Destination { get; }
	}
}
