using System;
using System.Diagnostics;

namespace Kinesense.Interfaces
{
	public class PreciseDateTime
	{
		private readonly Stopwatch _stopwatch;
		private readonly DateTime _baseTime;

		public PreciseDateTime()
		{
			_baseTime = DateTime.Now;
			_stopwatch = Stopwatch.StartNew();
		}

		/// Returns the current date and time, just like DateTime.UtcNow.
		public DateTime Now
		{
			get { return _baseTime + _stopwatch.Elapsed; }
		}
	}
}
