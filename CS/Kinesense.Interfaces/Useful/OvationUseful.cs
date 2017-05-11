using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Kinesense.Interfaces.Useful
{
    public class OvationUseful
    {
        /// <summary>
        /// Reads the gives short text file, returns "" on problem
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string ReadShortTextFile(string fileName)
        {
            return ReadShortTextFile(new FileInfo(fileName));
        }
        private static string ReadShortTextFile(FileInfo file)
        {
            string s = "";
            if (file.Exists)
            {
                try
                {
                    s = File.ReadAllText(file.FullName);
                }
                catch (Exception er) { }
            }
            return s;
        }

        /// <summary>
        /// Reads the given ovation text log and strips it of useless charachters
        /// </summary>
        /// <param name="location"></param>
        /// <returns>result or "" if problem</returns>
        public static string ReadOvationTxtLog(FileInfo file)
        {
            try
            {
                if (file.Exists)
                {
                    string x = ReadShortTextFile(file);
                    // clean up
                    x = x.Replace("\0", "");
                    return x;
                }
                else
                    return "";
            }
            catch (Exception er)
            {
                Kinesense.Interfaces.DebugMessageLogger.LogError(er);
            }
            finally
            {
                
            }
            return "";
        }

        /// <summary>
        /// Reads the given ovation text log and strips it of useless charachters
        /// </summary>
        /// <param name="location"></param>
        /// <returns>result or "" if problem</returns>
        public static string ReadOvationTxtLog(string location)
        {
            try
            {
                FileInfo f = new FileInfo(location);
                return ReadOvationTxtLog(f);
            }
            catch (Exception er)
            {
            }
            finally
            {
                
            }
            return "";
        }
    }
}
