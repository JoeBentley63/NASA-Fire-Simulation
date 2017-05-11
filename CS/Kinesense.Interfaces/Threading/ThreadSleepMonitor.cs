using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Kinesense.Interfaces.Threading
{
    public static class ThreadSleepMonitor
    {
        static Dictionary<string, double> _sleepCounts = new Dictionary<string, double>();
        static Dictionary<string, double> _sleepTotalTimes = new Dictionary<string, double>();
        static Dictionary<string, List<DateTime>> _sleeplog = new Dictionary<string, List<DateTime>>();
        static Dictionary<int, int> _sleephistogram = new Dictionary<int, int>();
        
        static int _SleepCount = 0;

        public static void Sleep(TimeSpan ts)
        {
            Sleep(ts.TotalMilliseconds);
        }

        public static bool TurnOffSleeps = false;

        private static object _sleeploglock = new object();

        public static bool CheckForMainThread()
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA &&
                !Thread.CurrentThread.IsBackground && !Thread.CurrentThread.IsThreadPoolThread && Thread.CurrentThread.IsAlive)
            {
                MethodInfo correctEntryMethod = Assembly.GetEntryAssembly().EntryPoint;
                StackTrace trace = new StackTrace();
                StackFrame[] frames = trace.GetFrames();
                for (int i = frames.Length - 1; i >= 0; i--)
                {
                    MethodBase method = frames[i].GetMethod();
                    if (correctEntryMethod == method)
                    {
                        return true;
                    }
                }
            }
            return false;
            // throw exception, the current thread is not the main thread...
        }

        public static void Sleep(double ms)
        {

#if DEBUG1

            try
            {
                lock (_sleeploglock)
                {
                    string stacktrace = new StackTrace().ToString();
                    if (!_sleepCounts.ContainsKey(stacktrace))
                        _sleepCounts.Add(stacktrace, 1);
                    else
                        _sleepCounts[stacktrace]++;

                    if (!_sleepTotalTimes.ContainsKey(stacktrace))
                        _sleepTotalTimes.Add(stacktrace, ms);
                    else
                        _sleepTotalTimes[stacktrace] += ms;

                    if (!_sleeplog.ContainsKey(stacktrace))
                        _sleeplog.Add(stacktrace, new List<DateTime> { DateTime.Now });
                    else
                        _sleeplog[stacktrace].Add(DateTime.Now);

                    _SleepCount++;
                    if (_SleepCount % 1000 == 0)
                        LogSleepData();

                    if (_sleephistogram.ContainsKey((int)ms))
                        _sleephistogram[(int)ms]++;
                    else
                        _sleephistogram.Add((int)ms, 1);
                }
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
#endif

            if (TurnOffSleeps)
                return;


            //if (ms <= 500)
            //    return;

            Thread.Sleep((int)ms);
        }

#if DEBUG
        public static void LogSleepData()
        {
            //order by most sleep calls
            //List<string> data = new List<string>();
            string data = "";

            List<KeyValuePair<string, double>> myList = _sleepCounts.ToList();

            myList.Sort(
                delegate (KeyValuePair<string, double> pair1,
                KeyValuePair<string, double> pair2)
                {
                    return pair1.Value.CompareTo(pair2.Value);
                }
            );

            foreach(var kvp in myList)
            {
                double tot = _sleepTotalTimes[kvp.Key];
                string s = string.Format("SleepCount {0} TotalTime {1} at {2}\n", kvp.Value, tot, kvp.Key);
                data += s;
            }

            foreach(var kvp in _sleephistogram)
            {
                data += string.Format("\n{0} - {1}", kvp.Key, kvp.Value);
            }

            DebugMessageLogger.LogEvent(data);
        }
#endif

    }
    
}
