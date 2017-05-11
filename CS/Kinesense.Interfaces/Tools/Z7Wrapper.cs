using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Kinesense.Interfaces
{
    public class Z7Wrapper
    {
        readonly string exeLoc = //@"C:\Program Files (x86)\7-Zip\7z.exe";
            Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "7za.exe"); 
        public Z7Wrapper()
        {
            if (!File.Exists(exeLoc))
                throw new IOException(exeLoc + " not found");
        }

        public delegate void ZipAsyncCallback(string output);

        private ZipAsyncCallback _ZipAsyncCallback;

        Thread _ZipAsyncProcessCommandlineReader;

        StreamReader _reader;

        Process _ZipProcess;

        public bool ZipFolder(string folder, string outputFile)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("a -tzip \"{0}\"", outputFile);
                sb.AppendFormat(" \"{0}\"", folder);

                //_ZipAsyncCallback = callback;

                DebugMessageLogger.LogEventLevel("Running: {0} {1}", 0, exeLoc, sb.ToString());
                ProcessStartInfo info = new ProcessStartInfo(exeLoc, sb.ToString());
                info.RedirectStandardOutput = true;
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                _ZipProcess = Process.Start(info);
                _reader = _ZipProcess.StandardOutput;

                ProcessOutputReader();

                return true;
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return false;
        }

        public string[] ZipFolder(string folder, string outputFile, int MaxSizeMB)
        {
            string[] list = new string[0];

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("a -tzip \"{0}\" -v{1}m", outputFile, MaxSizeMB);
                sb.AppendFormat(" \"{0}\"", folder);

                //_ZipAsyncCallback = callback;

                DebugMessageLogger.LogEventLevel("Running: {0} {1}", 0, exeLoc, sb.ToString());
                ProcessStartInfo info = new ProcessStartInfo(exeLoc, sb.ToString());
                info.RedirectStandardOutput = true;
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                _ZipProcess = Process.Start(info);
                _reader = _ZipProcess.StandardOutput;

                ProcessOutputReader();

                //find all files in the format outputFile.???
                var d = new DirectoryInfo(Path.GetDirectoryName(outputFile));
                FileInfo[] infos = d.GetFiles(Path.GetFileName(outputFile) + ".*");

                list = new string[infos.Length];

                if (infos.Length == 1)
                {
                    File.Move(infos[0].FullName, outputFile);
                    list[0] = outputFile;
                }
                else
                    for (int i = 0; i < infos.Length; i++)
                        list[i] = infos[i].FullName;

                DebugMessageLogger.LogEvent("Z7Wrapper - Zip Folder {0} to {1} Files of limit {2}MB", folder, list.Length, MaxSizeMB);
                
                return list;
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return list;
        }

        public bool Zip(FileInfo file, string outputFile)
        {
            return Zip(new string[] { file.FullName }, outputFile);
        }
        public bool Zip(string[] files, string outputFile)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("a -tzip \"{0}\"", outputFile);
                foreach (string file in files)
                    sb.AppendFormat(" \"{0}\"", file);

                //_ZipAsyncCallback = callback;

                DebugMessageLogger.LogEventLevel("Running: {0} {1}", 0, exeLoc, sb.ToString());
                ProcessStartInfo info = new ProcessStartInfo(exeLoc, sb.ToString());
                info.RedirectStandardOutput = true;
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                _ZipProcess = Process.Start(info);
                _reader = _ZipProcess.StandardOutput;

                ProcessOutputReader();

                return true;
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return false;
        }

        public void UnZip(string file, string outputfolder)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(exeLoc, string.Format("e {0} {1}", file, outputfolder));
               // info.RedirectStandardOutput = true;
                //info.CreateNoWindow = true;
               // info.UseShellExecute = false;
                _ZipProcess = Process.Start(info);
                _ZipProcess.WaitForExit(4000);
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }

        public bool ZipAsync(string[] files, string outputFile, ZipAsyncCallback callback)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("a -tzip \"{0}\"", outputFile);
                foreach (string file in files)
                    sb.AppendFormat(" \"{0}\"", file);

                _ZipAsyncCallback = callback;

                DebugMessageLogger.LogEventLevel("Running: {0} {1}", 0, exeLoc, sb.ToString());
                ProcessStartInfo info = new ProcessStartInfo(exeLoc, sb.ToString());
                info.RedirectStandardOutput = true;
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                _ZipProcess = Process.Start(info);
                _reader = _ZipProcess.StandardOutput;


                _ZipAsyncProcessCommandlineReader = new Thread(new ThreadStart(ProcessOutputReader));
                _ZipAsyncProcessCommandlineReader.Name = "Z7a.exe process commandline reader";
                _ZipAsyncProcessCommandlineReader.Start();

                return true;
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return false;
        }

        bool _continueReading = true;

        private void ProcessOutputReader()
        {
            StringBuilder sb = new StringBuilder();

            try
            {

                while (_continueReading && !_reader.EndOfStream)
                {
                    try
                    {
                        char[] block = new char[100];
                        _reader.ReadBlock(block, 0, 100);
                        string currentblock = new string(block);
                        //string line = previousblock + currentblock;
                        if (currentblock != null)
                        {
                            sb.AppendFormat("{0}", currentblock);
                        }
                    }
                    catch (Exception ee)
                    {
                        DebugMessageLogger.LogError(ee);
                    }

                }
            }
            catch(Exception eee)
            {
                DebugMessageLogger.LogError(eee);
            }
            
            if(_ZipAsyncCallback!=null)
                _ZipAsyncCallback(sb.ToString());
        }
    }
}
