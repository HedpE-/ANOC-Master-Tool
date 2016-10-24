using System;
namespace GMap.NET
{
    public class Point : IPositionTime
    {
        public double Easting { get; set; }
        public double Northing { get; set; }
        public DateTime Time { get; set; }
        public bool IsValid { get; set; }

        public Point()
        {
            IsValid = true;
        }

        public override string ToString()
        {
            //return string.Format("{3:HH:mm:ss} {0:0000}/{1:0000} {2:#.}", ((Easting % 100000) / 10.0), ((Northing % 100000) / 10.0), Altitude, Time);
            return string.Format("{0:HH:mm:ss} {1} {2:000000} {3:0000000} {4:0}", Time, Easting, Northing);
        }
    }
}
