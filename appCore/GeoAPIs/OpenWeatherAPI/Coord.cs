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
        public double lon { get; set; }
        public double lat { get; set; }

        public Coord() { }

        public Coord(JToken jToken)
        {
            lon = double.Parse(jToken.SelectToken("lon").ToString());
            lat = double.Parse(jToken.SelectToken("lat").ToString());
        }
    }
}
