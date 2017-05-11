using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Kinesense.Interfaces
{
    public static class StringTools
    {
        public static string CombineStringList(ICollection<string> list, string spacer)
        {
            string outstring = "";
            foreach (string s in list)
                outstring += s + spacer;
            return outstring;
        }

        public static string AnythingToCSV(params string[] vals)
        {
            string rtn = "";

            if (vals.Length == 0)
                return rtn;

            for (int i = 0; i < vals.Length - 1; i++)
                rtn += string.Format("\"{0}\",", vals[i].Replace("\"","''"));

            return rtn + string.Format("\"{0}\"", vals[vals.Length - 1].Replace("\"", "''"));

        }

        public static string AnythingToCSV_TabBased(params string[] vals)
        {
            string rtn = "";

            if (vals.Length == 0)
                return rtn;

            for (int i = 0; i < vals.Length - 1; i++)
                rtn += string.Format("\"{0}\"\t", vals[i].Replace("\"", "''").Replace("\t", "   "));

            return rtn + string.Format("\"{0}\"", vals[vals.Length - 1].Replace("\"", "''").Replace("\t", "   "));

        }

        public static string GetAfter(string instring, string afterthis)
        {
            int afterpos = instring.IndexOf(afterthis);
            if (afterpos == -1)
                return instring;

            return instring.Substring(afterpos + 1, instring.Length - afterpos - 1);
        }

        public static char[] StringSplitCharList = new char[] { ' ', '.', '*', ';', '"', '\'' };

        public static string Crop(string s, int maxlength)
        {
            if (String.IsNullOrEmpty(s))
                return "";
            return (s.Length > maxlength ? s.Substring(0, maxlength) : s);
        }

        public static string FindBetween(string content, string startTag, string endTag, int start)
        {
            int locStartTag = content.IndexOf(startTag, start);
            if (locStartTag == -1)
                return null;
            int locendtag = content.IndexOf(endTag, locStartTag + startTag.Length);

            return content.Substring(locStartTag + startTag.Length, locendtag - startTag.Length - locStartTag);
        }

        public static string InsertLinebreaks(string text, int lineCharLength, char[] breakat)
        {
            try
            {
                if (String.IsNullOrEmpty(text))
                    return "";

                if (text.Length <= lineCharLength)
                    return text;

                int searchBack = 5;
                int searchForward = 5;
                List<int> breakPos = new List<int>();

                for (int i = lineCharLength; i < text.Length; i += lineCharLength)
                {
                    //word wrap code.
                    int pos = text.IndexOfAny(breakat, i - searchBack, searchForward);
                    if (pos == -1)
                    {
                        pos = i;
                    }
                    breakPos.Add(pos);
                }
                breakPos.Add(text.Length);

                string[] lines = new string[breakPos.Count];
                int prev = 0;
                for (int i = 0; i < breakPos.Count; i++)
                {
                    lines[i] = text.Substring(prev, breakPos[i] - prev).TrimStart(' ', '\t');
                    prev = breakPos[i];
                }

                string output = lines[0];
                if (lines.Length > 1)
                    for (int i = 1; i < lines.Length; i++)
                        output += "\n" + lines[i];

                return output;
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return text;
        }

        public static string[] BreakToLines(string text, int lineCharLength)
        {
            if (String.IsNullOrEmpty(text))
                return new string[0];

            if (text.Length <= lineCharLength)
                return new string[] { text };

            int searchBack = 5;
            int searchForward = 5;
            List<int> breakPos = new List<int>();

            for (int i = lineCharLength; i < text.Length; i += lineCharLength)
            {
                //word wrap code.
                int pos = text.IndexOf(' ', i - searchBack, searchForward);
                if (pos == -1)
                {
                    pos = i;
                }
                breakPos.Add(pos);
            }
            breakPos.Add(text.Length);

            string[] lines = new string[breakPos.Count];
            int prev = 0;
            for (int i = 0; i < breakPos.Count; i++)
            {
                lines[i] = text.Substring(prev, breakPos[i] - prev).TrimStart(' ', '\t');
                prev = breakPos[i];
            }

            return lines;
        }

        public static string[] BreakToLinesWRTCarriageReturns(string text, int lineCharLength)
        {
            if (String.IsNullOrEmpty(text))
                return new string[0];

            string[] inParagraphs = text.Split(new string[1] { "\n" }, StringSplitOptions.None);
            int searchBack = 3;
            int searchForward = 3;
            int searchBack2 = 7;
            int searchForward2 = 5;
            List<string> lines = new List<string>();
            for (int j = 0; j < inParagraphs.Length; j++)
            {
                List<int> breakPos = new List<int>();

                int lastbreak = 0;
                int lastBreakLength = 0;

                for (int i = lineCharLength; i < inParagraphs[j].Length; i += lastBreakLength)
                {
                    //word wrap code.
                    int pos = inParagraphs[j].IndexOf(' ', i - searchBack, searchForward);
                    if (pos == -1)
                    {
                        pos = inParagraphs[j].IndexOf(' ', i - searchBack2, searchForward2);
                        if (pos == -1)
                            pos = i;
                    }
                    breakPos.Add(pos);
                    lastBreakLength = pos - lastbreak;
                    lastbreak = pos;

                }

                int prev = 0;
                for (int i = 0; i < breakPos.Count; i++)
                {
                    lines.Add(inParagraphs[j].Substring(prev, breakPos[i] - prev));
                    prev = breakPos[i];
                }
                if (j == inParagraphs.Length - 1)
                    lines.Add(inParagraphs[j].Substring(prev, inParagraphs[j].Length - prev));
                else
                    lines.Add(inParagraphs[j].Substring(prev, inParagraphs[j].Length - prev) + "\n");


            }
            return lines.ToArray();
        }

        /// <summary>
        /// take any string and encrypt it using MD5 then
        /// return the encrypted data 
        /// </summary>
        /// <param name="data">input text you will enterd to encrypt it</param>
        /// <returns>return the encrypted text as hexadecimal string</returns>
        public static string GetMD5HashData(string data)
        {
            //create new instance of md5
            MD5 md5 = MD5.Create();

            //convert the input text to array of bytes
            byte[] hashData = md5.ComputeHash(Encoding.Default.GetBytes(data));

            return ByteArrayUtils.ToHexString(hashData);

        }

        /// <summary>
        /// take any string and encrypt it using SHA1 then
        /// return the encrypted data
        /// </summary>
        /// <param name="data">input text you will enterd to encrypt it</param>
        /// <returns>return the encrypted text as hexadecimal string</returns>
        public static string GetSHA1HashData(string data)
        {
            //create new instance of md5
            SHA1 sha1 = SHA1.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(data));

            return ByteArrayUtils.ToHexString(hashData);
        }
    }

    public class StringHistogramBuilder
    {
        //public static string MakeHistogramOfStrings(string s)
        //{
        //    string formatsfound = "";
        //    Dictionary<string, int> countFiletypes = new Dictionary<string, int>();
        //    foreach (var v in e.ListOfVideoFilesFound)
        //    {
        //        string ext = System.IO.Path.GetExtension(v);
        //        if (countFiletypes.ContainsKey(ext))
        //        {
        //            int count = countFiletypes[ext];
        //            count++;
        //            countFiletypes[ext] = count;
        //        }
        //        else
        //        {
        //            countFiletypes.Add(ext, 1);
        //        }
        //    }

        //    foreach (var kvp in countFiletypes)
        //        formatsfound += string.Format("{0} ({1})", kvp.Key, kvp.Value);

        //    return formatsfound;
        //}
        Dictionary<string, int> countFiletypes = new Dictionary<string, int>();
        public StringHistogramBuilder()
        {
        }

        public void Add(string s)
        {
            if (countFiletypes.ContainsKey(s))
            {
                int count = countFiletypes[s];
                count++;
                countFiletypes[s] = count;
            }
            else
            {
                countFiletypes.Add(s, 1);
            }
        }

        public void Clear()
        {
            countFiletypes.Clear();
        }

        public override string ToString()
        {
            return GetSingleString("{0} ({1});");
        }

        public string GetSingleString(string pattern)
        {
            string formatsfound = "";
            foreach (var kvp in countFiletypes)
                formatsfound += string.Format(/*"{0} ({1})"*/pattern, kvp.Key, kvp.Value);

            return formatsfound;
        }
    }
}
