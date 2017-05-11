using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    public interface IVideoExporter : IDisposable
    {
        bool StartNewVideo(string filename, double framerate);
        int FrameWidth { get; }
        int FrameHeight { get; }
        double FrameRate { get; }
        void Close();
		//void AddFrame(System.Drawing.Bitmap bitmap);
		void AddFrame(System.Drawing.Bitmap bitmap, int newWidth, int newHeight);
        void PlayVideoExternally();
        string VideoFormat { get; }
		string Codec { get; set; }
		string[][] ListAvailableCodecs { get; }
		string[][] GenerateListAvailableCodecs(bool test);
        bool TestWriteCodec(string codec);
        int CompressionRate { get; set; }
    }
}
