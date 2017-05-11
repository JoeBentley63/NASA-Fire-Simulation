using Kinesense.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FireApp
{
    public class FireMap
    {
        public FireMap()
        {

        }

        FireCell[,] _firemap;
        public FireCell[,] FireCells { get { return _firemap; } set { _firemap = value; } }

        public int Width { get; set; }
        public int Height { get; set; }

        public FireMap(int width, int height)
        {
            Width = width;
            Height = height;

            _firemap = new FireCell[width, height];
        }

        public FireMap(int width, int height, bool random)
        {
            Width = width;
            Height = height;

            _firemap = new FireCell[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    _firemap[x, y] = FireCell.Random();
            RandomFireStarts();
        }
        public void RandomFireStarts()
        {

            //set fire to one point
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int countfires = rnd.Next(40) + 1;
            for (int r = 0; r < countfires; r++)
            {
                int x = rnd.Next(Width);
                int y = rnd.Next(Height);
                _firemap[x, y].FireIntensity = 256;
                _firemap[x, y].Fuel = 256;
            }

            WindXIntensity = 15;
            WindYIntensity = 15;
        }


        ByteArrayBitmap _bmp;
        public ByteArrayBitmap ToByteArrayBitmap()
        {
            if (_bmp == null || _bmp.Width != Width || _bmp.Height != Height)
                _bmp = new ByteArrayBitmap(Width, Height, PixelFormats.Bgr24);

            byte[] firecolor = new byte[] { 0, 0, 255 };

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    _bmp.SetColor(x, y, _firemap[x, y].GetColor());

            return _bmp;
        }

        public void PaintFire(ByteArrayBitmap bmp)
        {
            byte[] firecolor = new byte[] { 0, 0, 255 };

            byte[] watercolor = new byte[] { 255, 0, 0 };

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    if (_firemap[x, y].IsOnFire)
                        bmp.SetColor(x, y, firecolor);
                    else if (_firemap[x, y].Burned)
                    {
                        byte[] pixel = bmp.GetColor(x, y);
                        pixel[0] = (byte)(pixel[0] / 5);
                        pixel[1] = (byte)(pixel[1] / 5);
                        pixel[2] = (byte)(pixel[2] / 5);
                        bmp.SetColor(x, y, pixel);
                    }
                    else if (_firemap[x, y].Water > 125)
                        bmp.SetColor(x, y, watercolor);
                }            
        }        

        public double WindXIntensity { get; set; }
        public double WindYIntensity { get; set; }

        public double[] GenerateWindFactor()
        {
            // neighbouring factor is a measure of how much
            // a neighbour on fire affects the cell state
            const double ADJACENT_FACTOR = 1.0;
            const double DIAGONAL_FACTOR = 0.5;

            // Schema
            // 0 1 2
            // 3   4
            // 5 6 7

            double[] data = new double[8];
            data[0] = GetWindWeight(-1, -1, DIAGONAL_FACTOR);
            data[1] = GetWindWeight(0, -1, ADJACENT_FACTOR);
            data[2] = GetWindWeight(1, -1, DIAGONAL_FACTOR);

            data[3] = GetWindWeight(-1, 0, ADJACENT_FACTOR);
            data[4] = GetWindWeight(1, 0, ADJACENT_FACTOR);

            data[5] = GetWindWeight(-1, 1, DIAGONAL_FACTOR);
            data[6] = GetWindWeight(0, 1, ADJACENT_FACTOR);
            data[7] = GetWindWeight(1, 1, DIAGONAL_FACTOR);
            return data;
        }

        private double GetWindWeight(int i, int j, double neighbouringFactor)
        {
            double windProjection = this.WindXIntensity * i + this.WindYIntensity * j;
            double weight;
            if (windProjection < (int)WindTypes.NO_WIND)
            {
                weight = 0.1;
            }
            else if (windProjection < (int)WindTypes.LIGHT_WIND)
            {
                weight = 0.4;
            }
            else if (windProjection < (int)WindTypes.MODERATE_WIND)
            {
                weight = 0.7;
            }
            else
            {
                weight = 1.0;
            }

            return weight * neighbouringFactor;
        }

        //public double WindXIntensity { get; set; }
        //public double WindYIntensity { get; set; }

        //private double[] GenerateWindFactor()
        //{
        //    double[] data = new double[8];
        //    data[0] = 0.5; //0, 0
        //    data[1] = 1;   //0, 1
        //    data[2] = 0.5; //0, 2

        //    data[3] = 1;   //1, 0
        //    data[4] = 1;   //1, 2

        //    data[5] = 0.5; //2, 0
        //    data[6] = 1;   //2, 1
        //    data[7] = 0.5; //2, 2
        //    return data;
        //}

        private FireCell[] GetNeighbourhood(FireMap fm, int x, int y)
        {
            FireCell[] firecells = new FireCell[]
                    {fm._firemap[x-1, y-1],
                        fm._firemap[x-1, y],
                        fm._firemap[x-1, y+1],
                    fm._firemap[x, y-1]
                    ,fm._firemap[x, y+1],
                    fm._firemap[x+1, y-1],
                        fm._firemap[x+1, y],
                        fm._firemap[x+1, y+1]
                    };
            double[] wf = GenerateWindFactor();
            for (int i = 0; i < 8; i++)
                firecells[i].DirectionWeighting = wf[i];

            return firecells;
        }

        public void Update()
        {
            FireMap fm = this.Clone();

            for (int x = 1; x < Width - 1; x++)
                for (int y = 1; y < Height - 1; y++)
                {
                    _firemap[x, y].Adjust(GetNeighbourhood(fm, x, y));
                }
        }

        public FireMap Clone()
        {
            FireMap fm = new FireMap(Width, Height);
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    fm._firemap[x, y] = this._firemap[x, y].Clone();
            return fm;
        }
    }
}
