using System;
namespace GMap.NET
{
    public class LLPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Time { get; set; }
        public bool IsValid { get; set; }

        public LLPoint()
        {
            IsValid = true;
        }
    }
}
