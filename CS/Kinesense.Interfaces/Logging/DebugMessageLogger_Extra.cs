using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kinesense.Interfaces.Logging
{
    public static class DebugMessageLogger_Extra
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subName"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static int StartNewExtraLog(string subName, string reason)
        {
            try
            {
                int newLogNumber = GiveMeALogNumber;
                Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("DebugMessageLogger_Extra - Starting extra log for [" + reason + "] with log number " + newLogNumber.ToString(), 1);

                bool GotLock = false;

                try
                {
                    Monitor.TryEnter(ExtantLogs_Lock, 2000, ref GotLock);

                    if (!GotLock)
                    {
                        DebugMessageLogger.LogEventLevel("DebugMessageLogger_Extra - Could not get Lock on ExtantLogs Dictionary", 1);
                        return -1;
                    }

                    LogData LD = new LogData();

                    LD.number = newLogNumber;
                    LD.fullName = DebugMessageLogger.CurrentLogFile;
                    LD.fullName = LD.fullName.Remove(LD.fullName.Length - 4, 4);
                    LD.fullName = LD.fullName+"-EXTRA("+newLogNumber.ToString("000")+")-"+subName+".log";

                    ExtantLogs.Add(newLogNumber, LD);

                    WriteToFileWithTime("---------- LOG STARTED ----------"+"  REASON: (" + reason + ")", LD.fullName);

                }
                catch (Exception ker)
                {
                    Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                }
                finally
                {
                    if (GotLock)
                        Monitor.Exit(ExtantLogs_Lock);
                }

                return newLogNumber;
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                return -1;
            }                                    
        }

        #region Log Number System
        private static int _lastLogNumber = 1;
        private static object _lastLogNumber_Lock = new object();
        private static int GiveMeALogNumber
        {
            get
            {
                lock (_lastLogNumber_Lock)
                { 
                    _lastLogNumber++;
                    return _lastLogNumber;
                }
            }
        }
        #endregion

        private static Dictionary<int, LogData> ExtantLogs = new Dictionary<int, LogData>();
        private static object ExtantLogs_Lock = new object();

        public static void LogToExtraLog(int number, string ToLog)
        {
            try
            {
                bool GotLock = false;

                try
                {
                    Monitor.TryEnter(ExtantLogs_Lock, 2000, ref GotLock);

                    if (!GotLock)
                    {
                        DebugMessageLogger.LogEventLevel("DebugMessageLogger_Extra - Could not get Lock on ExtantLogs Dictionary", 1);
                        return;
                    }
                    if (!ExtantLogs.ContainsKey(number))
                    {
                        DebugMessageLogger.LogEventLevel("DebugMessageLogger_Extra - Could not find extra log number in ExtantLogs Dictionary - (" + number.ToString() + ")", 1);
                        return;
                    }

                    WriteToFileWithTime(ToLog, ExtantLogs[number].fullName);
                }
                catch (Exception ker)
                {
                    Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                }
                finally
                {
                    if (GotLock)
                        Monitor.Exit(ExtantLogs_Lock);
                }
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        public static void EndExtraLog(int number)
        {
            bool GotLock = false;

            try
            {
                Monitor.TryEnter(ExtantLogs_Lock,2000,ref GotLock);
                
                    if(!GotLock)
                    {
                        DebugMessageLogger.LogEventLevel("DebugMessageLogger_Extra - Could not get Lock on ExtantLogs Dictionary",1);
                        return;
                    }

                if (!ExtantLogs.ContainsKey(number))
                {
                    DebugMessageLogger.LogEventLevel("DebugMessageLogger_Extra - Could not find extra log number in ExtantLogs Dictionary - (" +number.ToString()+ ")",1);
                    return;
                }

                LogData LD = ExtantLogs[number];

                ExtantLogs.Remove(number);

                WriteToFileWithTime("---------- LOG CLOSED ----------", LD.fullName);

            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
            finally
            {
                if(GotLock)
                    Monitor.Exit(ExtantLogs_Lock);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void EndAllRemainingLogs()
        {
            try
            {
                bool GotLock = false;
                List<int> keys = new List<int>();
                DebugMessageLogger.LogEventLevel("DebugMessageLogger_Extra - Ending all remaining logs", 1);
                try
                {
                    Monitor.TryEnter(ExtantLogs_Lock, 2000, ref GotLock);

                    if (!GotLock)
                    {
                        DebugMessageLogger.LogEventLevel("DebugMessageLogger_Extra - Could not get Lock on ExtantLogs Dictionary", 1);
                        return;
                    }

                    keys = new List<int>(ExtantLogs.Keys);

                    DebugMessageLogger.LogEventLevel("DebugMessageLogger_Extra - Ending all remaining logs - " + keys.Count.ToString() + " found to end", 1);

                }
                catch (Exception ker)
                {
                    Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
                }
                finally
                {
                    if (GotLock)
                        Monitor.Exit(ExtantLogs_Lock);
                }

                foreach (int i in keys)
                    EndExtraLog(i);

            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
        }

        private static object _WriteLockObject = new object();
        private static void WriteToFileWithTime(string data, string destinationFile)
        {
            lock (_WriteLockObject)
            {
                File.AppendAllText(destinationFile, Kinesense.Interfaces.Useful.KinesenseInternationalTimeStringFormat.StandardLongTimeString(DateTime.Now) + " : " + data + "\n");
            }
        }

        private struct LogData
        {
            public int number;
            public string fullName;
        }
    }
}
