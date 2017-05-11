using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;

namespace Kinesense.Interfaces.Useful
{
    public static class SafeActionHelper
    {
        public static void NameThread(string name)
        {
            try
            {
                if(System.Threading.Thread.CurrentThread.Name == null)
                    System.Threading.Thread.CurrentThread.Name = name;
            }
            catch (Exception ee)
            {
            }
        }

        public static void Do(Action act, string taskname)
        {
            try
            {
                System.Threading.Thread.CurrentThread.Name = taskname;
                act.Invoke();
                act = null;
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }

        public static void DoTask(Action act, string taskname)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    System.Threading.Thread.CurrentThread.Name = taskname;
                    act.Invoke();
                    act = null;
                }
                catch (Exception ee)
                {
                    DebugMessageLogger.LogError(ee);
                }
            });
        }

        public static void DoTaskThenDispatch(Action act, string actionname, Dispatcher dispatcher, Action dispatchThisAction)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    System.Threading.Thread.CurrentThread.Name = actionname;
                    act.Invoke();
                    act = null;
                }
                catch (Exception ee)
                {
                    DebugMessageLogger.LogError(ee);
                }

                Do(dispatcher, dispatchThisAction);
                
            });
        }

        public static void DoLater(TimeSpan wait, Dispatcher dispatcher, Action act)
        {
            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.CurrentThread.Name = "Waiting Before Dispatch";
                Kinesense.Interfaces.Threading.ThreadSleepMonitor.Sleep(wait);
                Do(dispatcher, act);
            });
        }

        public static void DoTaskAndWait(Action act, string taskname)
        {
            AutoResetEvent ev = new AutoResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                try
                {

                    System.Threading.Thread.CurrentThread.Name = taskname;
                    act.Invoke();
                    ev.Set();
                    act = null;
                }
                catch (Exception ee)
                {
                    DebugMessageLogger.LogError(ee);
                }
            });
            ev.WaitOne();
        }

        public static void DoTaskAndWait(Action act, int msTimeout)
        {
            AutoResetEvent ev = new AutoResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                try
                {
                    System.Threading.Thread.CurrentThread.Name = act.Method.Name;
                    act.Invoke();
                    ev.Set();
                    act = null;
                }
                catch (Exception ee)
                {
                    DebugMessageLogger.LogError(ee);
                }
            });
            ev.WaitOne(msTimeout);
        }

        public static void DoImmediately(Dispatcher dispatcher, Action act)
        {
            dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    act.Invoke();
                    act = null;
                }
                catch (Exception ee)
                {
                    DebugMessageLogger.LogError(ee);
                }
            }));
        }

        public static void DoImmediatelyAndWait(Dispatcher dispatcher, Action act)
        {
            AutoResetEvent ev = new AutoResetEvent(false);
            dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    act.Invoke();
                    ev.Set();
                    act = null;
                }
                catch (Exception ee)
                {
                    DebugMessageLogger.LogError(ee);
                }
            }));
            ev.WaitOne();
        }

        public static void Do(Dispatcher dispatcher, Action act)
        {
            dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    act.Invoke();
                    act = null;
                }
                catch (Exception ee)
                {
                    DebugMessageLogger.LogError(ee);
                }
            }));
        }

        public static void MonitorDo(Dispatcher dispatcher, Action act, object monitorObj, int timeout)
        {
            if (Monitor.TryEnter(monitorObj, timeout))
            {
                AutoResetEvent ev = new AutoResetEvent(false);
                dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        act.Invoke();
                        act = null;
                        ev.Set();
                    }
                    catch (Exception ee)
                    {
                        DebugMessageLogger.LogError(ee);
                    }
                }));
                ev.WaitOne();
                Monitor.Exit(monitorObj);
            }
        }

        public static bool TryDo(Action action, int millisecondsTimeout)
        {
            var exceptionHappened = false;
            var thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    exceptionHappened = true;
                    DebugMessageLogger.LogError(ex);
                }
            });

            thread.Start();

            var completed = thread.Join(millisecondsTimeout);

            if (!completed || exceptionHappened)
            {
                thread.Abort();
                return false;
            }

            return true;
        }

        public static Action WrapAction(Action action)
        {
            return new Action(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch(Exception ee)
                {
                    DebugMessageLogger.LogError(ee);
                }
            });
        }

        public static bool TryDo<T>(Func<T> fun, int millisecondsTimeout, out T result)
        {
            var tResult = default(T);
            var exceptionHappened = false;

            var thread = new Thread(() =>
            {
                try
                {
                    tResult = fun();
                }
                catch (ThreadAbortException ex)
                {
                    exceptionHappened = true;
                    DebugMessageLogger.LogEvent("SaveActionHelper TryDo Timeout");
                    DebugMessageLogger.LogError(ex);
                }
                catch (Exception ex)
                {
                    exceptionHappened = true;
                    DebugMessageLogger.LogError(ex);
                }
            });

            thread.Start();

            var completed = thread.Join(millisecondsTimeout);

            result = tResult;

            if (!completed || exceptionHappened)
            {
                thread.Abort();
                return false;
            }

            return true;
        }
    }
}
