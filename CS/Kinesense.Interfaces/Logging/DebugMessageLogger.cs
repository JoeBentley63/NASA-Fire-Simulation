using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kinesense.Interfaces.Useful;
using System.Collections.Concurrent;
using System.Reflection;
using Kinesense.Interfaces.Logging;

namespace Kinesense.Interfaces
{
    /// <summary>
    /// Writes text logs and timestamps to a .log file for the current date.
    /// </summary>
    public static class DebugMessageLogger
    {
        public static void LogMemAndLinenumber()
        {
            StackFrame st = new StackFrame(1, true);
            float privateMB = Process.GetCurrentProcess().PrivateMemorySize64 / 1000000f;
            DebugMessageLogger.LogEvent("Memory Use {0}mb at {1}", privateMB, st);
        }

        public static void LogClick()
        {
            DebugMessageLogger.LogEvent("--USER CLICK--- {0}", new StackFrame(1, true));
        }

        public static string VersionTo2CharCode(Version ver)
        {
            try
            {
                return string.Format("{0}{1}", ConvIntToLowerCase(ver.Build), ConvIntToLowerCase(ver.Revision));
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return "-";
        }

        public static char ConvIntToLowerCase(int i)
        {
            //to rev value

            int val = (i % 26) + 97;
            return (char)val;
        }

        public static void LogTeamviewerID()
        {
            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Wow6432Node\TeamViewer");

                foreach (var subkeyname in key.GetSubKeyNames())
                {
                    var subkey = key.OpenSubKey(subkeyname);
                    var clientID = subkey.GetValue("ClientID");
                    DebugMessageLogger.LogEvent("Teamviewer {0} Client ID: {1}", subkeyname, clientID);
                }
            }
            catch (Exception ee)
            {
            }
        }

        private static DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }

        public static void LogApplicationArchitecture()
        {
            try
            {
                bool isx64 = Environment.Is64BitProcess;
                WriteToFile(string.Format("\r\n{0} Application is {1}", DateTime.Now.ToString(TimeStampFormat),
                    (isx64 ? "x64" : "x86")));
            }
            catch { }
        }

        public static string LogApplicationBuildDate()
        {
            string record = "";
            try
            {
                record = string.Format("\r\n{0} Application build on {1}", DateTime.Now.ToString(TimeStampFormat), RetrieveLinkerTimestamp());
                WriteToFile(record);
            }
            catch { }
            return record;
        }

        /// <summary>
        /// A measure of the ammount of logging to accept
        /// 0 = old (pre-level logging), 1 = normal, 2 = above normal, 3 = almost everything (typical debug setting), 4 = paranoid
        /// </summary>
        public static int EventLogLevel = 1;

        /// <summary>
        /// A measure of the ammount of logging to accept
        /// 0 = old (pre-level logging), 2 = paranoid
        /// </summary>
        public static int ErrorLogLevel = 5;


        static CultureInfo _currentculture = Thread.CurrentThread.CurrentCulture;
        static CultureInfo _logCulture = CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// immediately logs exception, including DateTime.Now timestamp, if the level of logging is high enough
        /// </summary>
        /// <param name="e"></param>
        public static void LogError(Exception e, int logLevel)
        {
            if (logLevel <= ErrorLogLevel)
            {
                DebugMessageLogger.LogError(e, null, logLevel);
            }
        }

        /// <summary>
        /// DEPRECIATED!!- Please use LogError(string s, int logLevel)
        /// immediately logs exception, including DateTime.Now timestamp, if the level of logging is high enough
        /// </summary>
        /// <param name="e"></param>
        public static void LogError(Exception e)
        {
            LogError(e, 0);
        }

        /// <summary>
        /// DEPRECIATED!!- Please use LogError(string s, string extra, int logLevel)
        /// immediately logs string with datetime.now timestamp in .log file, with extra info string
        /// </summary>
        /// <param name="s"></param>
        public static void LogError(Exception e, string extra)
        {
            LogError(e, extra, 0);
        }

        static int exceptionCount = 0;

        public const string TimeStampFormat = "HH:mm:ss.fff";

        public static string ExceptionFullString(Exception e)
        {
            //make sure log is in english
            Thread.CurrentThread.CurrentCulture = _logCulture;

            StringBuilder sb = new StringBuilder();

            try
            {
                Exception printException = e;
                while (printException != null)
                {
                    sb.AppendFormat("{0}\r\n", printException);
                    sb.AppendFormat("Source: {0}\r\n", printException.Source);
                    sb.AppendFormat("HasInnerException = {0}\r\n", (printException.InnerException != null));
                    printException = printException.InnerException;
                }
            }
            catch (Exception ee)
            {

            }
            Thread.CurrentThread.CurrentCulture = _currentculture;
            return sb.ToString();
        }

        private static Thread _diagnosticMemoryThread;
        private static bool _recordMemUsageRegularly = false;

        public static void StartDiagnosticMode()
        {
            try
            {
                _recordMemUsageRegularly = false;
                try
                {
                    if (_diagnosticMemoryThread != null)
                        _diagnosticMemoryThread.Abort();
                }
                catch (Exception eeee)
                {
                }
                _recordMemUsageRegularly = true;

                _diagnosticMemoryThread = new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            while (_recordMemUsageRegularly)
                            {
                                long totmem = GC.GetTotalMemory(true);
                                float privateMB = Process.GetCurrentProcess().PrivateMemorySize64 / 1000000f;
                                DebugMessageLogger.LogEvent("Diagnostic Mode: Private Bytes Usage: {0} MB, GC: {1} MB",
                                    privateMB.ToString("n"),
                                    (totmem / 1000000f).ToString("n"));

                                Kinesense.Interfaces.Threading.ThreadSleepMonitor.Sleep(10000);
                            }
                        }
                        catch (Exception eee)
                        {
                            DebugMessageLogger.LogError(eee);
                        }
                    }));
                _diagnosticMemoryThread.Start();
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }

        public static void StopDiagnosticMode()
        {
            try
            {
                _recordMemUsageRegularly = false;
                _diagnosticMemoryThread.Join();
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }

        public static ConcurrentQueue<string> LogQueue = new ConcurrentQueue<string>();

        /// <summary>
        /// immediately logs string with datetime.now timestamp in .log file, with extra info string
        /// </summary>
        /// <param name="s"></param>
        public static void LogError(Exception e, string extra, int logLevel)
        {
            if (logLevel <= ErrorLogLevel)
            {
                try
                {
                    ConvertExceptionToText(e, extra, logLevel, GetThreadSignature());
                }
                catch (Exception ee)
                {

                }
            }
        }

        private static void ConvertExceptionToText(Exception e, string extra, int logLevel, string threadSignature)
        {
            try
            {

                //make sure log is in english
                Thread.CurrentThread.CurrentCulture = _logCulture;

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("\r\n\r\n{0} [{1}] [{2}] {3}\r\n", DateTime.Now.ToString(TimeStampFormat), _instanceID, logLevel.ToString(), threadSignature);

                Exception printException = e;
                while (printException != null)
                {
                    sb.AppendFormat("{0}\r\n", printException);
                    sb.AppendFormat("Source: {0}\r\n", printException.Source);
                    sb.AppendFormat("HasInnerException = {0}\r\n", (printException.InnerException != null));
                    printException = printException.InnerException;
                }

                if (!String.IsNullOrEmpty(extra))
                    sb.AppendFormat("ExtraInfo: {0}\r\n", extra);

                sb.Append("\r\n\r\n");

                string message = sb.ToString();

                if (message != previousMessage)
                {
                    if (repeatcount > 1)
                        WriteToFile(string.Format("\r\nPrevious message written {0} times\r\n", repeatcount));

                    previousMessage = message;
                    repeatcount = 0;
                    WriteToFile(message);
                }
                else
                    repeatcount++;

                exceptionCount++;

                if (exceptionCount % 10 == 0)
                {
                    long totmem = 0;
                    Task t = Task.Factory.StartNew(
                        () =>
                        {
                            Thread.CurrentThread.Name = "Log Memory Use";
                            try
                            {
                                totmem = GC.GetTotalMemory(true);
                                float privateMB = Process.GetCurrentProcess().PrivateMemorySize64 / 1000000f;
                                DebugMessageLogger.LogEvent("Exception count: {0} Private Bytes Usage: {1} MB, GC: {2} MB",
                                    exceptionCount,
                                    privateMB.ToString("n"),
                                    (totmem / 1000000f).ToString("n"));
                            }
                            catch (Exception eeex)
                            {
                            }
                        }
                        );
                    //t.Wait(200);

                    //Calculate Memoryusage

                    //if (exceptionCount % 50 == 0)
                    //{
                    //    if ((DateTime.Now - _LastHDDCheckTime).TotalSeconds > 300)
                    //    {
                    //        _LastHDDCheckTime = DateTime.Now;
                    //        // check DB HDD
                    //        Task.Factory.StartNew(() =>
                    //            {
                    //                try
                    //                {
                    //                    long res = Useful.CheckHDDSize.HowMuchDBSpaceLeft();

                    //                    if (res != -1 && (res / (1 << 30)) < 1)
                    //                    {
                    //                        LogEventLevel("################## WARNING < 1 GB DB Space Available ####################", 0);
                    //                    }
                    //                    if (CheckHDDSize.DBDrive != null)
                    //                        LogEventLevel("Database Drive " + CheckHDDSize.DBDrive.RootDirectory + " has " + (res / (1024 * 1024)).ToString() + "Mb space left", 0);
                    //                }
                    //                catch (Exception er)
                    //                {
                    //                    Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                    //                }

                    //            });

                    //        // Check temp HDD
                    //        Task.Factory.StartNew(() =>
                    //        {
                    //            try
                    //            {
                    //                long res = Useful.CheckHDDSize.HowMuchTEMPSpaceLeft();

                    //                if (res != -1 && (res / (1 << 30)) < 1)
                    //                {
                    //                    LogEventLevel("################## WARNING < 1 GB DB Space Available ####################", 0);
                    //                }
                    //                if (CheckHDDSize.TEMPDrive != null)
                    //                    LogEventLevel("Temp Drive " + CheckHDDSize.TEMPDrive.RootDirectory + " has " + (res / (1024 * 1024)).ToString() + "Mb space left", 0);
                    //            }
                    //            catch (Exception er)
                    //            {
                    //                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
                    //            }

                    //        });
                    //    }
                    //}
                }
                else if (e is System.OutOfMemoryException
                    || (e.InnerException != null && e.InnerException is System.OutOfMemoryException))
                {
                    if ((DateTime.Now - _MostRecentMemoryTest).TotalSeconds > 60)
                    {
                        long totmem = 0;
                        Task t = Task.Factory.StartNew(() =>
                            {
                                totmem = GC.GetTotalMemory(true);
                                float privateMB = Process.GetCurrentProcess().PrivateMemorySize64 / 1000000f;
                                DebugMessageLogger.LogEvent("OutOfMemoryException Private Bytes Usage: {0} MB, GC: {1} MB",
                                    privateMB.ToString("n"),
                                    (totmem / 1000000f).ToString("n"));

                            });
                        t.Wait(200);
                        _MostRecentMemoryTest = DateTime.Now;
                    }
                    //Calculate Memoryusage

                }
            }
            catch (Exception ee)
            {
                //System.Windows.MessageBox.Show(ee.Message);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = _currentculture;
            }
        }

        static DateTime _MostRecentMemoryTest = DateTime.Now;

        private static DateTime _LastHDDCheckTime = DateTime.MinValue;

        /// <summary>
        /// immediately logs eveent, including DateTime.Now timestamp, if the level of logging is high enough
        /// </summary>
        /// <param name="e"></param>
        public static void LogEventLevel(string s, int logLevel)
        {
            if (logLevel <= EventLogLevel)
            {
                try
                {
                    WriteToFile(string.Format("\r\n{0} [{1}] [{2}] {3} {4}", DateTime.Now.ToString(TimeStampFormat), _instanceID, logLevel, GetThreadSignature(), s));
                }
                catch { }
            }
        }

        public static string GetThreadSignature()
        {
            if (System.Threading.Thread.CurrentThread.Name == null)
            {
            }
            return string.Format("[t-{0}-{1}]",
                System.Threading.Thread.CurrentThread.Name ?? "(unnamed)",
                System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// DONT USE - DOES NOTHIING
        /// 
        /// </summary>
        /// <param name="e"></param>
        public static void LogAltDebug(string s)
        {
            LogEvent(s);
        }

        /// <summary>
        /// DEPRECIATED!!- Please use LogEvent(string s, int logLevel)
        /// immediately logs string with datetime.now timestamp in .log file
        /// </summary>
        /// <param name="s"></param>
        public static void LogEvent(string s)
        {
            LogEventLevel(s, 0);
        }

        public static void LogEventLevel(int logLevel, string formatstring, params object[] objects)
        {
            if (logLevel <= ErrorLogLevel)
            {
                LogEventLevel(string.Format(formatstring, objects), logLevel);
            }
        }


        public static void LogEventLevel(string formatstring, int logLevel, params object[] objects)
        {
            if (logLevel <= ErrorLogLevel)
            {
                LogEvent(String.Format(formatstring, objects), logLevel);
            }
        }

        public static void LogEvent_DebugOnly(string formatstring, params object[] objects)
        {
#if DEBUG
            LogEvent(String.Format(formatstring, objects));
#endif
        }

        /// <summary>
        /// !!!DEPRECIATED -- Use LogEventLevel(string formatstring, int logLevel, params object[] objects) instead
        /// </summary>
        /// <param name="formatstring"></param>
        /// <param name="objects"></param>
        public static void LogEvent(string formatstring, params object[] objects)
        {
            LogEvent(String.Format(formatstring, objects));
        }

        private static object _LockObject = new object();

        public static bool UseLogQueue = true;

        public static int MaxQueueLength = 10;

        static AutoResetEvent _logAddedEvent = new AutoResetEvent(false);

        static Task _LogTask;
        //static object _LogThreadLock = new object();

        static int LogFrequency_MS = 1000;
        static int LogThreadTimeout_MS = 30000;

        static DateTime MostRecentLog = DateTime.Now;
        static bool _logThreadActive = false;

        public static void ExitLogThread()
        {
            Kinesense.Interfaces.Cleanup.GlobalThreadQuitter.IsExiting = true;
            _logAddedEvent.Set();
        }

        

        private static bool StartLogThread()
        {
            bool started = false;
            try
            {
                //if (Monitor.TryEnter(_LogThreadLock, 200) && _logThreadActive == false)
                if(_LogTask == null || _LogTask.IsCompleted)
                {
                    
                    _LogTask = new Task(() => LogThread());
                    //_LogTask. = "Error Logging Thread";
                    _LogTask.Start();
                    started = true;
                }
                else
                {

                    Console.WriteLine("Failed to enter queue {0} Exited {1} {2}",
                        (DateTime.Now - MostRecentLog).TotalSeconds, Kinesense.Interfaces.Cleanup.GlobalThreadQuitter.IsExiting, _logThreadActive);
                }
            }
            catch (Exception ee)
            {

            }

            return started;
        }

        private static void LogThread()
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine("Starting Thread For Debug Logs");
                _logThreadActive = true;
                do
                {
                    if (LogQueue.Count == 0)
                        _logAddedEvent.WaitOne(LogFrequency_MS);

                    if (LogQueue.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        string data = "";
                        while (LogQueue.Count > 0)
                        {
                            LogQueue.TryDequeue(out data);
                            sb.Append(data);
                        }

                        MostRecentLog = DateTime.Now;
                        data = sb.ToString();
                        File.AppendAllText(CurrentLogFile, data);
                    }
                }
                while ((DateTime.Now - MostRecentLog).TotalMilliseconds < LogThreadTimeout_MS
                &&
                !Kinesense.Interfaces.Cleanup.GlobalThreadQuitter.IsExiting);
            }
            catch (Exception ee)
            {
            }
            finally
            {
                Debug.WriteLine("Exiting Queue. LogThread ran for " + sw.Elapsed);
                _logThreadActive = false;
            }
        }

        private static bool _attemptingtoLaunch = false;
        private static void WriteToFile(string data)
        {
            if (UseLogQueue)
            {
                LogQueue.Enqueue(data);

                if (LogQueue.Count > MaxQueueLength)
                    _logAddedEvent.Set();

                if ((_LogTask == null || _LogTask.IsCompleted || _logThreadActive == false) && !_attemptingtoLaunch)
                {
                    _attemptingtoLaunch = true;
                    if (!StartLogThread())
                        Console.WriteLine(data);
                    _attemptingtoLaunch = false;
                }
            }
            else
            {
                lock (_LockObject)
                {
                    File.AppendAllText(CurrentLogFile, data);
                }
            }

        }

        static object _DebugLockObject = new object();

        private static void DebugWriteToFile(string data, string destinationFile)
        {
            lock (_DebugLockObject)
            {
                File.AppendAllText(destinationFile, data);
            }
        }

        private static void WriteToFile(string data, string destinationFile)
        {
            lock (_LockObject)
            {
                File.AppendAllText(destinationFile, data);
            }
        }

        public static string _instanceID = null;
        public static string InstanceID
        {
            get
            {
                if (string.IsNullOrEmpty(_instanceID))
                    _instanceID = Kinesense.Interfaces.ByteArrayUtils.ToHexString(
                        BitConverter.GetBytes((Int16)((new Random()).Next())));
                return _instanceID;
            }
            set
            {
                _instanceTime = DateTime.Now.Hour.ToString("00") + "-" + DateTime.Now.Minute.ToString("00") + "-" + DateTime.Now.Second.ToString("00");
                _instanceID = value;
            }
        }

        private static string _instanceTime = "";

        //public enum LogUsageEvent { StartSession, EndSession, StartProcess, EndProcess, StartForensic, EndForensic, ExportReport };

        //static object _UsageLockObject = new object();

        //public static void LogUsage(LogUsageEvent usageEvent)
        //{
        //    try
        //    {
        //        string folder = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\logs";
        //        if (Directory.Exists(folder) == false)
        //            Directory.CreateDirectory(folder);

        //        string filename = String.Format("{0}\\UsageLog.log", folder);

        //        lock (_UsageLockObject)
        //        {
        //            File.AppendAllText(filename, string.Format("{0} {1} [{2}]\r\n", usageEvent, DateTime.Now, _instanceID));
        //        }
        //    }
        //    catch (Exception ee)
        //    {
        //    }
        //}

        public static void WriteDataToFileInLogFolder(string filename, string dataformat, params object[] objects)
        {
            try
            {
                if (String.IsNullOrEmpty(LogsFolder))
                    SetLogsFolder();

                string path = System.IO.Path.Combine(LogsFolder, filename);

                File.AppendAllText(path, string.Format(dataformat, objects));
            }
            catch (Exception ee)
            {
                LogError(ee);
            }
        }

        static int _countLogEntries = 0;
        public static int MaxLogEntriesPerFile = 100000;
        static int _FileCount = 0;
        public static string CurrentLogFile
        {
            get
            {
                if (String.IsNullOrEmpty(LogsFolder))
                    SetLogsFolder();

                if (_countLogEntries > MaxLogEntriesPerFile)
                {
                    _FileCount++;
                    _countLogEntries = 0;
                }

                string datestring = String.Format(
                    "({0}-{1}-{2})_({3})_({4}-{5})",
                    DateTime.Today.Year.ToString("0000"),
                    DateTime.Today.Month.ToString("00"),
                    DateTime.Today.Day.ToString("00"),
                    _instanceTime,
                    InstanceID,
                    _FileCount);

                _countLogEntries++;

                return String.Format("{0}\\KinesenseLog{1}.log", LogsFolder, datestring);
            }
        }

        private static string CurrentLogFile_Alt
        {
            get
            {
                return CurrentLogFile.Replace(".log", "_Alt.log");
            }
        }

        public static bool SaveLogsLocally = false;

        public static void SetLogsFolder()
        {
            bool folder_set = false;
            try
            {
                if (SaveLogsLocally)
                {
                    string app_folder = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                    LogsFolder = System.IO.Path.Combine(app_folder, "Logs");
                }
                else
                {
                    LogsFolder = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "KinesenseData", "Logs");
                        //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kinesense", "Logs");
#if DEBUG
                    LogsFolder = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "KinesenseData", "Debug", "Logs");
                        //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kinesense", "Debug", "Logs");
#endif
                }

                if (Directory.Exists(LogsFolder) == false)
                    Directory.CreateDirectory(LogsFolder);

                _warningsFile = LogsFolder + Path.DirectorySeparatorChar + "warnings.txt";

                if (!File.Exists(_warningsFile))
                    File.Create(_warningsFile);

                ////test
                //string testfile = Path.Combine(folder, "test.log");
                //File.AppendAllText(testfile, "Test Admin rights... hmmm seems to have worked!");
                //File.Delete(testfile);
                folder_set = true;
            }
            catch (Exception ee)
            {
                try
                {
                    //alternative location for logs
                    LogsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                             "Kinesense", "Logs");
                    if (!Directory.Exists(LogsFolder))
                        Directory.CreateDirectory(LogsFolder);
                }
                catch (Exception eee)
                {
                    //message box?
                }
            }

        }

        public static string LogsFolder
        {
            get;
            set;
        }

        public static IList<string> GetAllLogs()
        {
            try
            {
                return Directory.GetFiles(LogsFolder);
            }
            catch (Exception ee)
            {
            }
            return null;
        }

        public static void RunCleanup(TimeSpan olderThan)
        {
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Thread.CurrentThread.Name = "Run Cleanup";
                        DateTime now = DateTime.Now;
                        DebugMessageLogger.LogEvent("Looking for log files older than {0} ago", olderThan);

                        List<string> paths = new List<string>();
                        paths.Add(LogsFolder);
                        foreach (var dir in Directory.GetDirectories(LogsFolder))
                            if(!dir.Contains("CommonViewingLogs")) //dont delete viewing logs
                                paths.Add(dir);

                        foreach (var dir in paths)
                            foreach (var file in Directory.GetFiles(dir, "*.log"))
                            {
                                TimeSpan howold = now - new FileInfo(file).LastWriteTime;
                                if (howold > olderThan)
                                {
                                    DebugMessageLogger.LogEvent("Deleting file {0} which is {1} days old", file, howold.Days);
                                    File.Delete(file);
                                }
                            }
                    }
                    catch (Exception ee)
                    {
                        DebugMessageLogger.LogError(ee);
                    }
                });
        }


        static string previousMessage = "";
        static int repeatcount = 0;


        public static void LogProcessOutput(Process proc, string name)
        {
            try
            {
                LogEvent(String.Format("{0} Started", name));
                while (!proc.StandardOutput.EndOfStream)
                {
                    LogEvent(String.Format("{0} Command Line: {1}", name, proc.StandardOutput.ReadLine()));
                }
                LogEvent(String.Format("{0} Ended", name));
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }

        //public static void ScreenShot(string guid)
        //{
        //    try
        //    {
        //        System.Drawing.Bitmap screen = new System.Drawing.Bitmap(
        //            System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
        //            System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height,
        //            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //        using (Graphics g = Graphics.FromImage(screen))
        //        {
        //            g.CopyFromScreen(
        //                System.Windows.Forms.Screen.PrimaryScreen.Bounds.X,
        //                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y, 0, 0,
        //                System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size,
        //                CopyPixelOperation.SourceCopy);
        //        }


        //        string path = @"\\Server\Shared\LogSpriteClasses\" + Path.GetFileName(System.Security.Principal.WindowsIdentity.GetCurrent().Name);

        //        if (!Directory.Exists(path))
        //            Directory.CreateDirectory(path);

        //        screen.Save(path + "\\" + guid + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        //    }
        //    catch { }
        //}

        public static void LogBinaryData(string description, byte[] data, int startpos, int endpos)
        {
            try
            {
                //turn data into a string 
                string sd = BitConverter.ToString(data, startpos, endpos).Replace("-", "");
                LogEvent(description + " Data: " + sd);
            }
            catch (Exception ee)
            {
            }
        }

        //public static void LogBinaryData(string description, byte[] data)
        //{
        //    try
        //    {
        //        //turn data into a string 
        //        string sd = BitConverter.ToString(data).Replace("-", "");
        //        LogEvent(description, sd);
        //    }
        //    catch (Exception ee)
        //    {

        //    }
        //}

        //private static object _logForLogData = new object();
        /// <summary>
        /// returns GUID
        /// </summary>
        /// <param name="commasep"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        //public static string LogData(IList<KeyValuePair<string, string>> commasep, string filename)
        //{
        //    System.Diagnostics.Trace.WriteLine("SpriteID=" + commasep[0].Value.ToString());
        //    string guid = "Failed";
        //    try
        //    {
        //        guid = Guid.NewGuid().ToString();
        //        commasep.Add(new KeyValuePair<string, string>("GUID", guid));
        //        StringBuilder sbvalues = new StringBuilder(), sbkeys = new StringBuilder();
        //        foreach (KeyValuePair<string, string> kvp in commasep)
        //        {
        //            sbvalues.Append(kvp.Value + ", ");
        //            sbkeys.Append(kvp.Key + ", ");
        //        }
        //        sbvalues.Append("\r\n");
        //        sbkeys.Append("\r\n");

        //        string path = @"\\Server\Shared\LogSpriteClasses\" + Path.GetFileName(System.Security.Principal.WindowsIdentity.GetCurrent().Name);

        //        if (!Directory.Exists(path))
        //            Directory.CreateDirectory(path);

        //        lock (_logForLogData)
        //        {
        //            string file = path + "\\" + filename + DateTime.Now.ToString("MM-yyyy") + ".csv";
        //            if (!File.Exists(file))
        //                File.AppendAllText(file, sbkeys.ToString());
        //            File.AppendAllText(file, sbvalues.ToString());
        //        }
        //    }
        //    catch { }
        //    return guid;
        //}

        public static void ZipAndSaveGivenFileToLog(string file)
        {
            try
            {
                FileInfo toZip = new FileInfo(file);
                if (!toZip.Exists)
                    return;

                string tempFile = LogsFolder + Path.DirectorySeparatorChar + "ziped.7z";

                if (File.Exists(tempFile))
                    File.Delete(tempFile);

                Z7Wrapper Z = new Z7Wrapper();
                Z.Zip(toZip, tempFile);


                InsertFileInLog(tempFile, file);

            }
            catch (Exception er)
            {
                LogError(er);
            }
        }

        /// <summary>
        /// Inserts the given file into the log
        /// </summary>
        /// <param name="filename"></param>
        private static void InsertFileInLog(string filename, string originFilename)
        {
            try
            {
                FileInfo fi = new FileInfo(filename);

                if (!fi.Exists)
                    return;

                if (fi.Length > 2000000)
                {
                    LogEventLevel("File too big to insert into log", 1);
                    return;
                }

                FileStream fs = fi.OpenRead();

                byte[] buffer = new byte[fi.Length];
                fs.Read(buffer, 0, (int)fi.Length);

                fs.Close();
                fs.Dispose();


                string pre = "######################################## File Embed - Filename: " + originFilename + "  - ####################################\n@#@\n";
                string post = "\n~@#@\n##############################################################################################################################\n\n";

                lock (_LockObject)
                {
                    using (fs = new FileStream(CurrentLogFile, FileMode.Append))
                    {
                        byte[] b1 = System.Text.Encoding.UTF8.GetBytes(pre);
                        byte[] b2 = System.Text.Encoding.UTF8.GetBytes(post);

                        fs.Write(b1, 0, b1.Length);
                        fs.Write(buffer, 0, buffer.Length);
                        fs.Write(b2, 0, b2.Length);
                        fs.Close();
                    }
                }

            }
            catch (Exception er)
            {
                LogError(er);
            }
        }




        // Warnings File

        private static string _warningsFile = "";
        private static object _warningsFileLock = new object();



        public static void LogWarning(string id, WarningReason reason)
        {
            LogWarning(id, reason, "");
        }

        public static void LogWarning(string id, WarningReason reason, string extraData)
        {
            try
            {
                if (Monitor.TryEnter(_warningsFile, 500))
                {
                    File.AppendAllText(_warningsFile, id + ";" + reason.ToString() + ";" + DateTime.Now.Ticks.ToString() + ";" + extraData + ";\n");
                    Monitor.Exit(_warningsFile);
                }
                else
                {
                    LogEventLevel("Could not get lock on warningsFile!!", 1);
                }
            }
            catch (Exception er)
            {
                LogError(er);
            }
        }

        private static int FindLastWarningLineNumber(string id, WarningReason reason, ref long ticks)
        {
            return FindLastWarningLineNumber(id, reason, ref ticks, "");
        }

        private static int FindLastWarningLineNumber(string id, WarningReason reason, ref long ticks, string extraData)
        {
            StreamReader sr = null;
            try
            {
                int lineNumber = -1;
                int count = 0;
                if (Monitor.TryEnter(_warningsFile, 500))
                {
                    sr = File.OpenText(_warningsFile);

                    string line = sr.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        if (line.Contains(id + ";" + reason.ToString() + ";"))
                        {
                            string[] s = line.Split(';');

                            if (s[3] == extraData)
                            {
                                ticks = long.Parse(s[2]);
                                lineNumber = count;
                            }
                        }

                        count++;
                        line = sr.ReadLine();
                    }
                    sr.Close();
                    sr.Dispose();
                }
                else
                {
                    LogEventLevel("Could not get lock on warningsFile!!", 1);
                }
                Monitor.Exit(_warningsFile);
                return lineNumber;
            }
            catch (Exception er)
            {
                Monitor.Exit(_warningsFile);
                if (sr != null)
                {
                    try
                    {
                        sr.Close();
                        sr.Dispose();
                    }
                    catch (Exception erm)
                    {
                        LogError(erm);
                    }
                }
                LogError(er);
            }

            return -1;
        }

        public static DateTime GetLastWarningOf(string id, WarningReason reason)
        {
            return GetLastWarningOf(id, reason, "");
        }

        public static DateTime GetLastWarningOf(string id, WarningReason reason, string extraData)
        {
            try
            {

                long ticks = -1;
                FindLastWarningLineNumber(id, reason, ref ticks, extraData);
                if (ticks != -1)
                    return new DateTime(ticks);
                else
                    return DateTime.MinValue;

            }
            catch (Exception er)
            {
                LogError(er);
            }
            return DateTime.MinValue;
        }
    }
}