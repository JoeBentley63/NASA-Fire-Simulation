using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Kinesense.Interfaces.Useful
{
    public class NetworkTime
    {
        /// <summary>
        /// returns the deviation of the system clock from network time
        /// </summary>
        /// <returns></returns>
        public static TimeSpan? GetDeviationOfSystemClockFromNetworkTime(int maxWaitInMS)
        {
            var tokenSource2 = new CancellationTokenSource();
            CancellationToken ct = tokenSource2.Token;

            Task<TimeSpan?> t1 = Task.Factory.StartNew(() => Kinesense.Interfaces.Useful.NetworkTime.GetDeviationOfSystemClockFromNetworkTime(ct),ct);

            if (t1.Wait(maxWaitInMS))
            {
                return t1.Result;
            }
            else
            {
                tokenSource2.Cancel();
                return null;
            }
        }



        /// <summary>
        /// returns the deviation of the system clock from network time
        /// </summary>
        /// <returns></returns>
        public static TimeSpan? GetDeviationOfSystemClockFromNetworkTime(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return null;
            try
            {
                DateTime realNow = GetNetworkTime("uk.pool.ntp.org");
                return realNow - DateTime.Now;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            if (ct.IsCancellationRequested)
                return null;
            try
            {
                DateTime realNow = GetNetworkTime("europe.pool.ntp.org");
                return realNow - DateTime.Now;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            if (ct.IsCancellationRequested)
                return null;
            try
            {
                DateTime realNow = GetNetworkTime("ntp2d.mcc.ac.uk");
                return realNow - DateTime.Now;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            if (ct.IsCancellationRequested)
                return null;
            try
            {
                DateTime realNow = GetNetworkTime("time-a.nist.gov");
                return realNow - DateTime.Now;
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            return null;
        }



        /// <summary>
        /// Gets the current DateTime from time-a.nist.gov.
        /// </summary>
        /// <returns>A DateTime containing the current time.</returns>
        public static DateTime GetNetworkTime()
        {
            return GetNetworkTime("time-a.nist.gov");
        }

        /// <summary>
        /// Gets the current DateTime from <paramref name="ntpServer"/>.
        /// </summary>
        /// <param name="ntpServer">The hostname of the NTP server.</param>
        /// <returns>A DateTime containing the current time.</returns>
        public static DateTime GetNetworkTime(string ntpServer)
        {
            IPAddress[] address = Dns.GetHostEntry(ntpServer).AddressList;

            if (address == null || address.Length == 0)
                throw new ArgumentException("Could not resolve ip address from '" + ntpServer + "'.", "ntpServer");

            IPEndPoint ep = new IPEndPoint(address[0], 123);

            return GetNetworkTime(ep);
        }

        /// <summary>
        /// Gets the current DateTime form <paramref name="ep"/> IPEndPoint.
        /// </summary>
        /// <param name="ep">The IPEndPoint to connect to.</param>
        /// <returns>A DateTime containing the current time.</returns>
        public static DateTime GetNetworkTime(IPEndPoint ep)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            s.Connect(ep);

            byte[] ntpData = new byte[48]; // RFC 2030 
            ntpData[0] = 0x1B;
            for (int i = 1; i < 48; i++)
                ntpData[i] = 0;

            s.Send(ntpData);
            s.Receive(ntpData);

            byte offsetTransmitTime = 40;
            ulong intpart = 0;
            ulong fractpart = 0;

            for (int i = 0; i <= 3; i++)
                intpart = 256 * intpart + ntpData[offsetTransmitTime + i];

            for (int i = 4; i <= 7; i++)
                fractpart = 256 * fractpart + ntpData[offsetTransmitTime + i];

            ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);
            s.Close();

            TimeSpan timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);

            DateTime dateTime = new DateTime(1900, 1, 1);
            dateTime += timeSpan;

            TimeSpan offsetAmount = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
            DateTime networkDateTime = (dateTime + offsetAmount);

            return networkDateTime;
        }
    }
}
