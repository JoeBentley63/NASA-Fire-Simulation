using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Kinesense.Interfaces.Clarification
{
    public interface IClarificationUIBase
    {
        event EventHandler ResultImageSet;
        event ErrorStateDelegate SetErrorStateEvent;

        ByteArrayBitmap GetResultImage();
        void SetSourceImage(ByteArrayBitmap val);
        void ApplyChanges();
    }
}
