using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Kinesense.Interfaces.Useful
{
    class GetWindowsErrorCodeText
    {
        public static string GetErrorText(int code)
        {
            string rtn = "failed to retrieve text for error " + code.ToString();

            try
            {
                ProcessStartInfo info = new ProcessStartInfo("net", "helpmsg " + code.ToString());
                info.RedirectStandardOutput = true;
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                Process p = Process.Start(info);
                StringBuilder sb = new StringBuilder();
                while (!p.StandardOutput.EndOfStream)
                {
                    sb.Append(p.StandardOutput.ReadToEnd());
                }

                rtn = sb.ToString().Replace('\n', ' ').Replace('\r', ' ');
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
            return rtn;
        }
    }
}
