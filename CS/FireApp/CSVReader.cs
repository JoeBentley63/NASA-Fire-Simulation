using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp
{
    public class CSVReader
    {
        public static int[,] Read(string file)
        {
            string[] line = File.ReadAllLines(file);
            int height = line.Length;
            int width = line[0].Count(f => f == ',');

            int[,] data = new int[width, height];
            for (int y = 0; y < height; y++)
            {
                string[] parts = line[y].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < width; x++)
                    data[x, y] = int.Parse(parts[x]);
            }

            return data;
        }
    }
}
