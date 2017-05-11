using System;
using System.Collections.Generic;

namespace Kinesense.Interfaces
{
	public interface IImageForensics
	{
		ByteArrayBitmap ComputeModifiedFrame(ByteArrayBitmap bitmap);
	}
}
