using System;
using System.Windows;

namespace Kinesense.Interfaces
{
    public interface ICloseRequester
    {
        event EventHandler CloseRequested;
        void PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e);
        Window Owner { get; set; }
    }
}
