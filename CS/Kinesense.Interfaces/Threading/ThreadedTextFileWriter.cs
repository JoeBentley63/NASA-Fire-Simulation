using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kinesense.Interfaces.Useful
{
    public class ThreadedTextFileWriter : IDisposable
    {
        private FileInfo _file;
        // private FileStream _fStream;
        private StreamWriter _fWriter;
        private BackgroundWorker _fSave;
        private Queue<string> _buffer = new Queue<string>();
        private object _bufferKey = new object();
        private DateTime _lastReportTime;
        private DateTime _startTime;

        /// <summary>
        /// Sets up the writer, if given a file that exists, 
        /// </summary>
        /// <param name="fileToWrite"></param>
        public ThreadedTextFileWriter(string fileToWrite)
        {
            try
            {
                if (File.Exists(fileToWrite))
                    File.Delete(fileToWrite);

                _file = new FileInfo(fileToWrite);
                DirectoryInfo d = _file.Directory;
                if (!d.Exists)
                    d.Create();

                _fWriter = _file.CreateText();

                if (_fWriter != null)
                {
                    _fWriter.AutoFlush = false;
                    _fSave = new BackgroundWorker();
                    _fSave.DoWork += fSave_doWork;
                    _fSave.RunWorkerCompleted += fSave_runWorkComplete;
                    _fSave.WorkerSupportsCancellation = true;
                    _fSave.RunWorkerAsync();
                }
                _lastReportTime = DateTime.Now;
                _startTime = _lastReportTime;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
        }

        /// <summary>
        /// Main body of the object doing the work of saving
        /// </summary>
        /// <param name="o"></param>
        /// <param name="d"></param>
        private void fSave_doWork(object o, DoWorkEventArgs d)
        {
            BackgroundWorker B = o as BackgroundWorker;

            while (!B.CancellationPending)
            {
                Kinesense.Interfaces.Threading.ThreadSleepMonitor.Sleep(500);
                if (_bufferEnquireSize() > 0)
                {
                    List<string> S = _bufferDequeueAll();

                    foreach (string s in S)
                    {
                        _fWriter.WriteLine(s);
                    }
                    _fWriter.Flush();
                }
            }
        }

        /// <summary>
        /// Ensures cleanup and correct messages reported
        /// </summary>
        /// <param name="o"></param>
        /// <param name="r"></param>
        private void fSave_runWorkComplete(object o, RunWorkerCompletedEventArgs r)
        {
            if (r.Error != null)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogEvent("ThreadedTextFileWriter.fSave_doWork encountered and error");
                Kinesense.Interfaces.DebugMessageLogger.LogError(r.Error);
            }
            else if (r.Cancelled)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogEvent("ThreadedTextFileWriter.fSave_doWork was cancelled");
            }
            else
            {
                Kinesense.Interfaces.DebugMessageLogger.LogEvent("ThreadedTextFileWriter.fSave_doWork exited naturally");
            }
            _fWriter.Flush();
            _fWriter.Close();
            _fWriter.Dispose();
        }


        /// <summary>
        /// Threadsafe enqueue string
        /// </summary>
        /// <param name="toEnc"></param>
        private void _bufferEnqueue(string toEnc)
        {
            lock (_bufferKey)
            {
                _buffer.Enqueue(toEnc);
            }
        }

        /// <summary>
        /// Threadsafe dequeue string
        /// </summary>
        /// <returns></returns>
        private string _bufferDequeue()
        {
            lock (_bufferKey)
            {
                if (_buffer.Count > 0)
                    return _buffer.Dequeue();

                return "";
            }
        }

        /// <summary>
        /// Threadsafe dump of entire queue
        /// </summary>
        /// <returns></returns>
        private List<string> _bufferDequeueAll()
        {
            List<string> S = new List<string>();
            lock (_bufferKey)
            {
                while (_buffer.Count > 0)
                {
                    S.Add(_buffer.Dequeue());
                }
            }
            return S;
        }

        /// <summary>
        /// Threadsafe size check
        /// </summary>
        /// <returns></returns>
        private int _bufferEnquireSize()
        {
            lock (_bufferKey)
                return _buffer.Count;
        }


        /// <summary>
        /// Adds a simple report to the system
        /// </summary>
        /// <param name="S"></param>
        public void AddSimpleReport(string S)
        {
            this._bufferEnqueue(S);
        }

        /// <summary>
        /// Adds the report preceeded by the current time
        /// </summary>
        /// <param name="S"></param>
        public void AddTimeStampedReport(string S)
        {
            this._bufferEnqueue(DateTime.Now.ToLongTimeString() + " - " + S);
        }

        /// <summary>
        /// Adds a report with a time stame representing the time difference from the last report
        /// </summary>
        /// <param name="S"></param>
        public void AddTimeIntervalStampedReport(string S)
        {
            this._bufferEnqueue((DateTime.Now - _lastReportTime).TotalMilliseconds.ToString() + " - " + S);
            _lastReportTime = DateTime.Now;
        }

        /// <summary>
        /// Adds a report with a time stame representing the duration since the begining of the report
        /// </summary>
        /// <param name="S"></param>
        public void AddTimeSinceStartStampedReport(string S)
        {
            this._bufferEnqueue((DateTime.Now - _startTime).TotalMilliseconds.ToString() + " - " + S);
        }

        /// <summary>
        /// Cancels thread, flushes bufferes and cleans up
        /// </summary>
        public void TerminateRecording()
        {
            if (_fSave != null && _fSave.IsBusy && !_fSave.CancellationPending)
            {
                _fSave.CancelAsync();
            }
            else if (_fWriter != null)
            {
                _fWriter.Flush();
                _fWriter.Close();
                _fWriter.Dispose();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            TerminateRecording();
        }

        #endregion
    }

    public class ThreadedTextFileWriter_ThrowawayVersion : IDisposable
    {
        private FileInfo _file;
        // private FileStream _fStream;
        private StreamWriter _fWriter;
        private BackgroundWorker _fSave;
        private Queue<string> _buffer = new Queue<string>();
        private object _bufferKey = new object();
        private DateTime _lastReportTime;
        private DateTime _startTime;
        private bool FinishWritingAndCleanUp = false;
        private bool RequestFinishWritingAndCleanUp = false;
        private string _fileToWrite = "";
        private bool _goDormant = false;
        private bool _gonDormant = false;



        /// <summary>
        /// Sets up the writer, if given a file that exists, 
        /// </summary>
        /// <param name="fileToWrite"></param>
        public ThreadedTextFileWriter_ThrowawayVersion(string fileToWrite)
        {
            try
            {
                _fileToWrite = fileToWrite;

                if (File.Exists(fileToWrite))
                    File.Delete(fileToWrite);

                _file = new FileInfo(fileToWrite);
                DirectoryInfo d = _file.Directory;
                if (!d.Exists)
                    d.Create();

                _fWriter = _file.CreateText();

                if (_fWriter != null)
                {
                    _fWriter.AutoFlush = false;
                    _fSave = new BackgroundWorker();
                    _fSave.DoWork += fSave_doWork;
                    _fSave.RunWorkerCompleted += fSave_runWorkComplete;
                    _fSave.WorkerSupportsCancellation = true;
                    _fSave.RunWorkerAsync();
                }
                _lastReportTime = DateTime.Now;
                _startTime = _lastReportTime;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
        }

        private void RestartSaveThread()
        {
            try
            {
                Kinesense.Interfaces.DebugMessageLogger.LogEvent("Sleeping TTFW_TAV Waking up....", 1);
                _file = new FileInfo(_fileToWrite);
                if (_fWriter != null)
                {
                    _fWriter.AutoFlush = false;
                    _fSave = new BackgroundWorker();
                    _fSave.DoWork += fSave_doWork;
                    _fSave.RunWorkerCompleted += fSave_runWorkComplete;
                    _fSave.WorkerSupportsCancellation = true;
                    _fSave.RunWorkerAsync();
                }
            }
            catch (Exception ker)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(ker);
            }
        }

        /// <summary>
        /// Main body of the object doing the work of saving
        /// </summary>
        /// <param name="o"></param>
        /// <param name="d"></param>
        private void fSave_doWork(object o, DoWorkEventArgs d)
        {
            BackgroundWorker B = o as BackgroundWorker;
            int count = 0;

            while (!B.CancellationPending && !FinishWritingAndCleanUp)
            {
                if (_bufferEnquireSize() > 0)
                {
                    List<string> S = _bufferDequeueAll();

                    foreach (string s in S)
                    {
                        _fWriter.WriteLine(s);
                    }
                    _fWriter.Flush();

                    count = 0;
                }
                Kinesense.Interfaces.Threading.ThreadSleepMonitor.Sleep(100);
                count++;

                if (count > 30)
                {
                    this._goDormant = true;

                    break;
                }
            }
        }

        /// <summary>
        /// Ensures cleanup and correct messages reported
        /// </summary>
        /// <param name="o"></param>
        /// <param name="r"></param>
        private void fSave_runWorkComplete(object o, RunWorkerCompletedEventArgs r)
        {
            if (r.Error != null)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogEvent("ThreadedTextFileWriter.fSave_doWork encountered and error");
                Kinesense.Interfaces.DebugMessageLogger.LogError(r.Error);
            }
            else if (r.Cancelled)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogEvent("ThreadedTextFileWriter.fSave_doWork was cancelled");
            }
            else
            {
                if (_goDormant)
                    Kinesense.Interfaces.DebugMessageLogger.LogEvent("ThreadedTextFileWriter.fSave_doWork exited -- going dormant");
                else
                    Kinesense.Interfaces.DebugMessageLogger.LogEvent("ThreadedTextFileWriter.fSave_doWork exited naturally");
            }
            _fWriter.Flush();
            _fWriter.Close();
            _fWriter.Dispose();
            if (_goDormant)
            {
                _gonDormant = true;
                _goDormant = false;
            }
        }


        /// <summary>
        /// Threadsafe enqueue string
        /// </summary>
        /// <param name="toEnc"></param>
        private void _bufferEnqueue(string toEnc)
        {
            if (RequestFinishWritingAndCleanUp)
                throw new Exception("WriteAllAndCleanUp has already been called");


            lock (_bufferKey)
            {
                _buffer.Enqueue(toEnc);
            }
        }

        /// <summary>
        /// Threadsafe dequeue string
        /// </summary>
        /// <returns></returns>
        private string _bufferDequeue()
        {
            lock (_bufferKey)
            {
                if (_buffer.Count > 0)
                    return _buffer.Dequeue();
                //else
                //{
                //    if (RequestFinishWritingAndCleanUp)
                //        FinishWritingAndCleanUp = true;
                //}

                return "";
            }
        }

        /// <summary>
        /// Threadsafe dump of entire queue
        /// </summary>
        /// <returns></returns>
        private List<string> _bufferDequeueAll()
        {
            List<string> S = new List<string>();
            lock (_bufferKey)
            {
                while (_buffer.Count > 0)
                {
                    S.Add(_buffer.Dequeue());
                }
                //if (RequestFinishWritingAndCleanUp)
                //    FinishWritingAndCleanUp = true;
            }
            return S;
        }

        /// <summary>
        /// Threadsafe size check
        /// </summary>
        /// <returns></returns>
        private int _bufferEnquireSize()
        {
            if (Monitor.TryEnter(_bufferKey, 500))
            {
                int b = _buffer.Count;
                Monitor.Exit(_bufferKey);
                return b;
            }
            else
            {
                Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("ThreadedTextFileWriter_Throwaway._bufferEnquireSize() Got Deadlocked and exited with non-threadsafe answer", 1);
                return _buffer.Count;
            }
        }


        /// <summary>
        /// Adds a simple report to the system
        /// </summary>
        /// <param name="S"></param>
        public void AddSimpleReport(string S)
        {
            if (RequestFinishWritingAndCleanUp)
                throw new Exception("WriteAllAndCleanUp has already been called");

            this._bufferEnqueue(S);


            int count = 0;
            while (this._goDormant && count < 20)
            {
                Kinesense.Interfaces.Threading.ThreadSleepMonitor.Sleep(100);
                count++;
            }

            if (count == 20)
                Kinesense.Interfaces.DebugMessageLogger.LogEvent("TTFW_TAV has failed in a dormant recycle - text not written");

            if (_gonDormant)
                RestartSaveThread();
        }

        /// <summary>
        /// Adds the report preceeded by the current time
        /// </summary>
        /// <param name="S"></param>
        public void AddTimeStampedReport(string S)
        {
            if (RequestFinishWritingAndCleanUp)
                throw new Exception("WriteAllAndCleanUp has already been called");

            AddSimpleReport(DateTime.Now.ToLongTimeString() + " - " + S);
        }

        /// <summary>
        /// Adds a report with a time stame representing the time difference from the last report
        /// </summary>
        /// <param name="S"></param>
        public void AddTimeIntervalStampedReport(string S)
        {
            if (RequestFinishWritingAndCleanUp)
                throw new Exception("WriteAllAndCleanUp has already been called");

            AddSimpleReport((DateTime.Now - _lastReportTime).TotalMilliseconds.ToString() + " - " + S);
            _lastReportTime = DateTime.Now;
        }

        /// <summary>
        /// Adds a report with a time stame representing the duration since the begining of the report
        /// </summary>
        /// <param name="S"></param>
        public void AddTimeSinceStartStampedReport(string S)
        {
            if (RequestFinishWritingAndCleanUp)
                throw new Exception("WriteAllAndCleanUp has already been called");

            AddSimpleReport((DateTime.Now - _startTime).TotalMilliseconds.ToString() + " - " + S);
        }

        /// <summary>
        /// Cancels thread, flushes bufferes and cleans up
        /// </summary>
        public void TerminateRecording()
        {
            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("TTFW_TAV.TerminateRecording()", 3);
            WriteAllAndCleanUp();
            if (_fSave != null && _fSave.IsBusy && !_fSave.CancellationPending)
            {
                _fSave.CancelAsync();
            }
            else if (_fWriter != null)
            {
                _fWriter.Flush();
                _fWriter.Close();
                _fWriter.Dispose();
            }
        }

        /// <summary>
        /// Call this on abandonment to make the thread clean itself up then exit
        /// </summary>
        public void WriteAllAndCleanUp()
        {
            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("TTFW_TAV.RequestFinishWritingAndCleanUp set to True", 3);
            FinishWritingAndCleanUp = true;
            RequestFinishWritingAndCleanUp = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("TTFW_TAV.Dispose() called", 3);
            TerminateRecording();
        }

        #endregion
    }
}
