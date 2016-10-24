using System;

namespace GMap.NET
{
    public interface IPosition
    {
        double Easting { get; }
        double Northing { get; }
    }
}
