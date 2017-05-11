using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Interfaces
{
    public interface IMultiChannelMediaFileMetadataWrapper
    {
        List<int> ChannelsRecorded { get; set; }
        int ChoosenChannel { get; set; }
    }
}
