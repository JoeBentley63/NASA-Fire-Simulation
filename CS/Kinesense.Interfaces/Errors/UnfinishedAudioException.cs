using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Errors
{
    public class UnfinishedAudioException : Exception
    {
        public UnfinishedAudioException() { }

        public UnfinishedAudioException(string message) : base(message) { }

        public UnfinishedAudioException(string message, Exception inner) : base(message, inner) { }
    }
}
