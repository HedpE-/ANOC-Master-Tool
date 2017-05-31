using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OpenWeatherAPI
{
    public class Coord
    {
        double lon, lat;
        
        public double Longitude { get { return lon; } }
        public double Latitude { get { return lat; } }

        public Coord(JToken coordData)
        {
//        	try {
            lon = double.Parse(coordData.SelectToken("lon").ToString());
//        	}
//        	catch (Exception e) {
//        		lon = double.Parse(coordData.SelectToken("coord.lon").ToString());
//        	}
            lat = double.Parse(coordData.SelectToken("lat").ToString());
        }
    }
}
