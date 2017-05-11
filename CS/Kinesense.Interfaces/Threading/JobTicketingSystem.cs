using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Threading
{
    public class JobTicketingSystem
    {
        public static void TestTicket(string sourcename, int queuelength, ref bool stopGeneratingFrames)
        {
            // Video recording is now a single thread for all imports, and hence has a queue.
            // This code pauses the imports if the save queue grows beyond a set limit, as the save
            // queue eats ram. It is designed in a circular ticket system so no one source gets
            // undue prominence in the save system.
            //
            if (queuelength > 2)
            {
                DateTime queueEntry = DateTime.Now;

               DebugMessageLogger.LogEvent("ImportingVideoPlayer.GenerateFrames - Wait Requested Source={0}, Current Queue Length={1}",
                        sourcename, queuelength);

                int ticket = VQueueTicket_getTicket();
                while (!stopGeneratingFrames)
                {
                    Kinesense.Interfaces.Threading.ThreadSleepMonitor.Sleep(1000);
                    if (queuelength < 4 && VQueueTicket_checkTicket(ticket))
                        break;
                    DebugMessageLogger.LogEventLevel(1,
                        "ImportingVideoPlayer.GenerateFrames - Wait Continues Source={0}, Current Queue Length={1}, Ticket={2}, Been Queuing For {3} seconds",
                        sourcename, queuelength, ticket, (DateTime.Now - queueEntry).TotalSeconds);
                }
                if (stopGeneratingFrames)
                {
                    DebugMessageLogger.LogEventLevel(1, "ImportingVideoPlayer.GenerateFrames - Stop generating requested whilst in queue. Ticket={0} to be surrounded", ticket);
                    VQueueTicket_surrenderTicket(ticket);
                }
            }
        }

        // Ticket system to prevent one import thread dominating when import is suspended due
        // to the save gob queue length getting too big

        private static int _vQueueTicket_no = -1;
        private static int _vQueueTicket_serving = 0;
        private static object _vQueueTicket_key = new object();
        private static List<int> _vQueueTicket_surrendedTickets = new List<int>();

        /// <summary>
        /// Get a ticket to be in line for restart
        /// </summary>
        /// <returns></returns>
        public static int VQueueTicket_getTicket()
        {
            lock (_vQueueTicket_key)
            {
                _vQueueTicket_no++;
                return _vQueueTicket_no;
            }
        }
        /// <summary>
        /// Tries to redeem ticket for wait queue
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static bool VQueueTicket_checkTicket(int ticket)
        {
            lock (_vQueueTicket_key)
            {
                // check surrended ticket list
                if (_vQueueTicket_surrendedTickets.Count > 0)
                {
                    for (int i = 0; i < _vQueueTicket_surrendedTickets.Count; i++)
                    {
                        if (_vQueueTicket_surrendedTickets[i] == _vQueueTicket_serving)
                        {
                            Kinesense.Interfaces.DebugMessageLogger.LogEventLevel("VideoReocorder - Surrended Ticket=" + _vQueueTicket_surrendedTickets[i].ToString() + " processed", 1);
                            _vQueueTicket_surrendedTickets.RemoveAt(i);
                            _vQueueTicket_serving++;
                            i = 0;
                        }
                    }
                }
                // process requested ticket
                if (ticket == _vQueueTicket_serving)
                {
                    _vQueueTicket_serving++;
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public static void VQueueTicket_surrenderTicket(int ticket)
        {
            DebugMessageLogger.LogEventLevel("VideoReocorder - Ticket=" + ticket.ToString() + " to be surrended", 1);
            if (!VQueueTicket_checkTicket(ticket))
            {
                DebugMessageLogger.LogEventLevel("VideoReocorder - Ticket=" + ticket.ToString() + " added to surrended list", 1);
                _vQueueTicket_surrendedTickets.Add(ticket);
            }
            else
            {
                DebugMessageLogger.LogEventLevel("VideoReocorder - Ticket=" + ticket.ToString() + " surrended and processed", 1);
            }
        }
    }
}
