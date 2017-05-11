using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Errors
{
    public class LostCameraStreamException : Exception
    {
        public LostCameraStreamException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public LostCameraStreamException(string message)
            : base(message)
        {
        }

        public new Exception InnerException { get; set; }
        public bool LossIsPermanent { get; set; }
    }
}
