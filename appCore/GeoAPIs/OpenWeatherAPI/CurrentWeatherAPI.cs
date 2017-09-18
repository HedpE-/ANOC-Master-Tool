using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenWeatherAPI.CurrentWeather
{
    public class Main
    {
        public double temp { get; set; }
        public double pressure { get; set; }
        public int humidity { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        [JsonIgnore]
        TemperatureObj _temperature;
        [JsonIgnore]
        public TemperatureObj temperature
        {
            get
            {
                if (_temperature == null)
                    _temperature = new TemperatureObj(temp, temp_min, temp_max);

                return _temperature;
            }
            private set { _temperature = value; }
        }
        //[JsonIgnore]
        //public System.Drawing.Image HumidityPicture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.WeatherPicturesLocation.FullName + @"\hygrometer.png");
        //[JsonIgnore]
        //public System.Drawing.Image TemperaturePicture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.WeatherPicturesLocation.FullName + @"\thermometer.png");

    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public double message { get; set; }
        public string country { get; set; }
        int sunrise { get; set; }
        int sunset { get; set; }
        [JsonIgnore]
        public DateTime Sunrise { get { return convertUnixToDateTime(sunrise); } }
        [JsonIgnore]
        public DateTime Sunset { get { return convertUnixToDateTime(sunset); } }

        private DateTime convertUnixToDateTime(double unixTime)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(unixTime).ToLocalTime();
        }
    }

    public class CurrentWeatherQuery
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    public class CurrentWeatherItem
    {
        public bool ValidRequest { get; set; }
        public DateTime DataTimestamp { get; set; }
        public string Town { get; set; }
        
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }

        public CurrentWeatherItem() { }

        public CurrentWeatherItem(CurrentWeatherQuery query, string town)
        {
            coord = query.coord;
            weather = query.weather;
            @base = query.@base;
            main = query.main;
            visibility = query.visibility;
            wind = query.wind;
            clouds = query.clouds;
            dt = query.dt;
            sys = query.sys;
            id = query.id;
            name = query.name;
            cod = query.cod;
            DataTimestamp = DateTime.Now;
            ValidRequest = cod == 200;
            Town = town;
        }
    }
}
