using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp
{
    public class FireCell
    {
        public FireCell Clone()
        {
            return new FireCell
            {
                FireIntensity = this.FireIntensity,
                Fuel = this.Fuel,
                Temperature = this.Temperature,
                Water = this.Water,
            };
        }

        public byte[] GetColor()
        {
            byte[] color = new byte[] { 0, 0, 0 };
            if (IsOnFire)
            {
                color[2] = 255;
            }
            else
            {
                if (Water > 0)
                    color[0] = (byte)(Water < 256 ? Water : 255);
                if (Fuel > 0)
                    color[1] = (byte)(Fuel < 256 ? Fuel : 255);
                if (Temperature > 0)
                    color[2] = (byte)(Temperature < 256 ? Temperature : 255);
            }

            return color;
        }

        public static FireCell Random()
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return new FireCell()
            {
                FireIntensity = 0,//rnd.Next(256),
                Fuel = rnd.Next(256),
                Temperature = rnd.Next(100),
                Water = rnd.Next(0)
            };

            //return new FireCell()
            //{
            //    FireIntensity = 0,
            //    Fuel = rnd.Next(256),
            //    Temperature = 100,
            //    Water = rnd.Next(256)
            //};

        }
        public double Water { get; set; }
        public double Temperature { get; set; }
        public double FireIntensity { get; set; }
        public double DirectionWeighting { get; set; }

        public bool IsOnFire { get { return FireIntensity > 0 && Fuel > 0; } }
        public double Fuel { get; set; }
        public bool Burned { get; set; }

        public void Adjust(FireCell[] neighbours)
        {
            if (IsOnFire)
            {
                Fuel -= 5;// + Fuel/5);
            }
            else if (Fuel < 1)
            {
                FireIntensity = 0;
            }
            else
            {
                double fireintensity = 0;
                foreach (FireCell n in neighbours)
                {
                    fireintensity += (n.FireIntensity * n.DirectionWeighting);
                }
                fireintensity -= Water;
                if (fireintensity > 0)
                {
                    Water -= 10;
                    if (Water < 0)
                        Water = 0;
                }
                fireintensity = (int)fireintensity;
                this.FireIntensity = fireintensity > 0 ? fireintensity : 0;
                if (this.IsOnFire)
                    Burned = true;
            }
        }
    }
}
