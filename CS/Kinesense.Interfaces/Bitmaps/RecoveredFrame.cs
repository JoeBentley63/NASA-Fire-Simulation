using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    /// <summary>
    /// A new class that will be returned from plugins when they return a frame
    /// </summary>
    [Serializable]
    public class RecoveredFrame : IDisposable
    {

        public int FrameID { get; set; }


        public RecoveredFrame(VideoFrame vf, int source, string notes)
        {
            Frame = vf;
            Source = source;
            Notes = notes;
        }
        public RecoveredFrame(VideoFrame vf, int source)
        {
            Frame = vf;
            Source = source;
        }
        public RecoveredFrame(bool noMorFrames)
        {
            NoMoreFrames = noMorFrames;
        }

        /// <summary>
        /// The Video frame data
        /// </summary>
        public VideoFrame Frame;
        /// <summary>
        /// The video frame source (0,1,2 etc..)
        /// </summary>
        public int Source;
        /// <summary>
        /// Notes if there arte any issues
        /// </summary>
        public string Notes;
        /// <summary>
        /// If the video has ended
        /// </summary>
        public bool NoMoreFrames;

        public Exception Exception;

        #region IDisposable Members
        public void Dispose()
        {
            this.Dispose(true);
        }
        private bool disposed = false;
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }

                if (Frame != null)
                    Frame.Dispose();

                this.disposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}