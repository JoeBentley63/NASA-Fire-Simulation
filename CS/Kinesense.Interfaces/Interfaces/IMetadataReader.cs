using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinesense.Interfaces.EventArguments;

namespace Kinesense.Interfaces.Classes
{
    public class SiraViewMetadataReaderBase
    {

        public static SiraViewMetadataReaderConstructor SiraViewConstructorDelegate;  
        public SiraViewMetadataReaderBase(string path)
        {
        }

        public virtual DateTime? StartTime { get; set; }

        public virtual TimeSpan? Duration { get; set; }

        public virtual string Codec { get; set; }

        public virtual int ChannelCount { get; set; }

        public event EventHandler<SiraViewLoadingProgressEventArgs> SiraViewProgress;

        protected void OnProgress(SiraViewLoadingProgressEventArgs args)
        {
            if (SiraViewProgress != null)
            {
                SiraViewProgress(this, args);
            }
        }
    }

    public interface IPluginRunOnLoad
    {
        void RunOnLoad();
    }

    public delegate SiraViewMetadataReaderBase SiraViewMetadataReaderConstructor(string filepath);
}
