using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Kinesense.Interfaces.Classes
{
    public static class LogPreLoader
    {
        private static bool _logPreLoadHasBeenStarted = false;
        private static BackgroundWorker _logPreLoadWorker;
        private static string _rerport;

        public static void StartLogPreLoad()
        {
            if (!_logPreLoadHasBeenStarted)
            {
                _logPreLoadHasBeenStarted = true;
                _logPreLoadWorker = new BackgroundWorker();
                _logPreLoadWorker.WorkerSupportsCancellation = true;
                _logPreLoadWorker.DoWork += new DoWorkEventHandler(_logPreLoadWorker_DoWork);
                _logPreLoadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_logPreLoadWorker_RunWorkerCompleted);
                _logPreLoadWorker.RunWorkerAsync();
            }
        }


        public static void StopLogPreLoad()
        {
            if (_logPreLoadWorker != null && _logPreLoadWorker.IsBusy)
                _logPreLoadWorker.CancelAsync();
        }

        private static void AddToReport(string s)
        {
            _rerport += s + Environment.NewLine;
        }
        private static void AddToReport(string[] s)
        {
            _rerport += s + Environment.NewLine;
        }
        private static void AddToReport(string s, string[] ss, bool multiline)
        {
            if (multiline)
            {
                foreach (string g in ss)
                    _rerport += s + g + Environment.NewLine;
            }
            else
            {
                string toP = "";
                foreach (string g in ss)
                    toP += g + ", ";

                toP = toP.TrimEnd(new char[] { ' ', ',' });

                _rerport += s + toP + Environment.NewLine;
            }
        }


        private static void _logPreLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                AddToReport("");
                AddToReport("####################### SYS REPORT ###################");
                //
                AddToReport("OSVersion: " + Environment.OSVersion);
                AddToReport("Version: " + Environment.Version);
                AddToReport("Is64BitOperatingSystem: " + Environment.Is64BitOperatingSystem);
                AddToReport("ProcessorCount: " + Environment.ProcessorCount);
                AddToReport("TickCount: " + Environment.TickCount + "(" + Kinesense.Interfaces.Useful.KinesenseInternationalTimeStringFormat.ShortTimeSpanString(new TimeSpan(0,0,0,0,Environment.TickCount)) + ")");
                AddToReport("SystemDirectory: " + Environment.SystemDirectory);
                AddToReport("Logical Drive: ", Environment.GetLogicalDrives(), false);
                //                              
                AddToReport("MachineName: " + Environment.MachineName);
                AddToReport("UserName: " + Environment.UserName);
                //
                AddToReport("CommandLine: " + Environment.CommandLine);
                AddToReport("Is64BitProcess: " + Environment.Is64BitProcess);
                //
                AddToReport("######################################################");
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
        }


        static void _logPreLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                // Cancelled, do nothing
            }

            else if (!(e.Error == null))
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(e.Error);
            }

            else
            {
                Kinesense.Interfaces.DebugMessageLogger.LogEventLevel(_rerport, 1);
            }
        }




    }
}
