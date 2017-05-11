using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Bitmaps
{
    using FramePacketIndexEntry = KeyValuePair<DateTime, int>;

    public class FramePacketIndex : Dictionary<DateTime, int>
    {
        public new void Add(DateTime dt, int i)
        {
            base.Add(dt, i);
            _dateList.Add(dt);
        }

        private List<DateTime> _dateList = new List<DateTime>();
        public List<DateTime> DateList { get { return _dateList; } }

        public byte[] ToBytes()
        {
            List<FramePacketIndexEntry> list = this.ToList();

            int entrylength = 8 /* sizeof DateTime */ + sizeof(int);
            int length = //2 // starting '--'
                + (list.Count * entrylength); //index
                                              //+ sizeof(int); //position in file of index start

            byte[] bytes = new byte[length];

            for (int pos = 0, i = 0; pos < length && i < this.Count; pos += entrylength, i++)
            {
                Array.Copy(BitConverter.GetBytes(list[i].Key.ToBinary()), 0, bytes, pos, 8);
                Array.Copy(BitConverter.GetBytes(list[i].Value), 0, bytes, pos + 8, 4);
            }

            return bytes;
        }

        public static FramePacketIndex GetIndexFromBytes(byte[] bytes, int start, int indexlen)
        {
            int entrylength = 8 /* sizeof DateTime */ + sizeof(int);

            FramePacketIndex index = new FramePacketIndex();
            for (int pos = start; pos < bytes.Length && pos < indexlen + start; pos += entrylength)
            {
                if (pos + entrylength > bytes.Length)
                    break;
                DateTime dt = DateTime.FromBinary(BitConverter.ToInt64(bytes, pos));
                int x = BitConverter.ToInt32(bytes, pos + 8);
                index.Add(dt, x);
            }

            return index;
        }

        public static FramePacketIndex MakeDummyIndex(DateTime start, int numFrames, int fps)
        {
            FramePacketIndex ind = new FramePacketIndex();
            double interframedelay = 1000d / (double)fps;
            for(int i = 0; i < numFrames; i++)
            {
                ind.Add(start.AddMilliseconds(i * fps), i);
            }

            return ind;
        }
    }
}
