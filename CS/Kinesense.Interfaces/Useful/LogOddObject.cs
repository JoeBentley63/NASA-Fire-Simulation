
namespace Kinesense.Interfaces.Useful
{
    public static class LogOddObject
    {
        public static void logArray(short[,] data, string name)
        {
            string op = name + "\n\n";

            for (int i = 0; i < data.GetLength(1); i++)
            {             //add 2 because second stage process lags behind
                for (int j = 0; j < data.GetLength(0); j++)
                {
                    op += data[j, i].ToString("000") + ", ";
                }
                op += "\n";
            }
            op += "\n\n";
            Kinesense.Interfaces.DebugMessageLogger.LogEvent(op);
        }

        public static void logArray(byte[,] data, string name)
        {
            string op = name + "\n\n";

            for (int i = 0; i < data.GetLength(1); i++)
            {             //add 2 because second stage process lags behind
                for (int j = 0; j < data.GetLength(0); j++)
                {
                    op += data[j, i].ToString("000") + ", ";
                }
                op += "\n";
            }
            op += "\n\n";
            Kinesense.Interfaces.DebugMessageLogger.LogEvent(op);
        }

        /// <summary>
        /// Logs a 1d array as a 2d array
        /// </summary>
        /// <param name="data"></param>
        /// <param name="name"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="wdenh"></param>
        public static void logArray(short[] data, string name, int width, int height, bool wdenh)
        {
            string op = name + "\n\n";

            if (wdenh)
            {
                int count = 0;
                while (count < data.Length)
                {
                    for (int i = 0; i < width; i++)
                    {
                        op += data[count].ToString("000") + ", ";
                        count++;
                    }
                    op += "\n";
                }
                op += "\n\n";
                Kinesense.Interfaces.DebugMessageLogger.LogEvent(op);
            }
            else
            {
                int count = 0;
                int count2 = 0;
                short[,] newData = new short[width, height];
                while (count < data.Length)
                {
                    for (int i = 0; i < height; i++)
                    {
                        newData[count, i] = data[count];
                        count++;
                    }
                    count2++;
                }

                logArray(newData, name);
            }
        }


    }
}
