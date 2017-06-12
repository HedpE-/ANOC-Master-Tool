using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenWeatherAPI;
using OpenWeatherAPI.CurrentWeather;
using OpenWeatherAPI.Forecast;

namespace OpenWeatherAPI
{
    public class WeatherItem
    {
        public CurrentWeatherItem CurrentWeather { get; set; }
        public ForecastItem Forecast5D3H { get; set; }
    }
}
