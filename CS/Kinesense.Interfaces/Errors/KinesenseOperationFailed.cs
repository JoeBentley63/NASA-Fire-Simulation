    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Errors
{
        public class KinesenseOperationFailed : System.Exception
        {
            // The default constructor needs to be defined
            // explicitly now since it would be gone otherwise.

            public KinesenseOperationFailed()
            {
            }

            public KinesenseOperationFailed(string message) : base(message)
            {
            }

            public KinesenseOperationFailed(string message, Exception innerException): base(message, innerException)
            {
            }

            public KinesenseOperationFailed(string problem,string noteToUser, string noteToLog)
            {
                this.Problem = problem;
                this.NoteToUser = noteToUser;
                this.NoteToLog = noteToLog;
            }

            public KinesenseOperationFailed(string message, string problem, string noteToUser, string noteToLog)
                : base(message)
            {
                this.Problem = problem;
                this.NoteToUser = noteToUser;
                this.NoteToLog = noteToLog;
            }

            public KinesenseOperationFailed(string message, Exception innerException, string problem, string noteToUser, string noteToLog)
                : base(message, innerException)
            {
                this.Problem = problem;
                this.NoteToUser = noteToUser;
                this.NoteToLog = noteToLog;
            }

            private string _problem;
            public string Problem
            {
                get { return this._problem; }
                set { this._problem = value; }
            }

            private string _noteToUser;
            public string NoteToUser
            {
                get { return this._noteToUser; }
                set { this._noteToUser = value; }
            }

            private string _noteToLog;
            public string NoteToLog
            {
                get { return this._noteToLog; }
                set { this._noteToLog = value; }
            }
        }

    
}
