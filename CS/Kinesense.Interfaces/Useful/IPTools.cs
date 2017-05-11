using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Kinesense.Interfaces;
using System.IO;

namespace Kinesense.Interfaces.Useful
{
    public static class IPTools
    {
        /// <summary>
        /// Gets the internal IP address
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            IPHostEntry host;
            string localIP = "?";

            try
            {
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                    }
                }
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee, "Failed to get IP address. Perhaps Computer is not online");
            }
            return localIP;
        }

        public static string GetPublicIP()
        {
            String direction = "?";
            DebugMessageLogger.LogEvent("Requesting External IP Address from http://checkip.dyndns.org/");
            try
            {
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    direction = stream.ReadToEnd();
                }

                //Search for the ip in the html
                int first = direction.IndexOf("Address: ") + 9;
                int last = direction.LastIndexOf("</body>");
                direction = direction.Substring(first, last - first);

                DebugMessageLogger.LogEvent("My IP is " + direction);
            }
            catch (Exception e)
            {
                DebugMessageLogger.LogError(e, "Failed to get IP address. Perhaps Computer is not online");
            }
            return direction;
        }
    }
}
