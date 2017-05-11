using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.EventArguments
{
    namespace StatusUpdateEventArgs
    {
        /// <summary>
        /// A simple status update event that reports a decimal
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public delegate void SimpleStatusUpdateEventHandler(object o, SimpleStatusUpdateEventArgs e);
        public class SimpleStatusUpdateEventArgs : EventArgs
        {
            public readonly decimal status;

            public SimpleStatusUpdateEventArgs(decimal Status)
            {
                status = Status;
            }
        }

        /// <summary>
        /// A more complex reporting event that allows a generic action - status - note report with a timestamp.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public delegate void AdvancedStatusUpdateEventHandler(object o, AdvancedStatusUpdateEventArgs e);
        public class AdvancedStatusUpdateEventArgs : EventArgs
        {
            public readonly string action;
            public readonly decimal percent;
            public readonly string note;
            public readonly DateTime when;

            public AdvancedStatusUpdateEventArgs(string Action, decimal Percent, string Note)
            {
                action = Action;
                percent = Percent;
                note = Note;
                when = DateTime.Now;
                    
            }
        }
    }
}
