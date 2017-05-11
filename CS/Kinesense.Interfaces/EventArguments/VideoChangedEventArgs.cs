using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    public class VideoChangedEventArgs
    {
        public int VideoID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool hasBeenDeleted = false;

        /// <summary>
        /// Read this as has had sprites deleted, but will work for any change
        /// </summary>
        public bool hasHadSpritesChanged = false;
    }
}
