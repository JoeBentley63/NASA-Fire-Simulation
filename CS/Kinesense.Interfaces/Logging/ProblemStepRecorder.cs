using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Logging
{
    public class ProblemStepRecorder
    {
        private static int Count = 0;
        public static void StartRecording()
        {
            Process.Start("psr.exe",
                string.Format("/start /output \"{0}\" /sc 1 /maxsc 100", 
                System.IO.Path.ChangeExtension(DebugMessageLogger.CurrentLogFile, 
                string.Format("ScreenShots-{0}.zip", Count++))));
        }

        public static void StopRecording()
        {
            Process.Start("psr.exe", "/stop");
        }

        //psr.exe /start /output "C:\Users\mark\Desktop\demopsr.zip" /sc 1 /maxsc 100 /recordpid 4120 https://annoyedadmin.wordpress.com/2011/12/02/windows-7-psr/

        //psr.exe /stop
    }
}
