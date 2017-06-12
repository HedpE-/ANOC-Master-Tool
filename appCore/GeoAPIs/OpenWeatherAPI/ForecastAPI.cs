using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenWeatherAPI.Forecast
{
    public class Main
    {
        public double temp { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public double pressure { get; set; }
        public double sea_level { get; set; }
        public double grnd_level { get; set; }
        public int humidity { get; set; }
        public double temp_kf { get; set; }

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
    }

    public class Sys
    {
        public string pod { get; set; }
    }

    public class ForecastList
    {
        public int dt { get; set; }
        public Main main { get; set; }
        public List<Weather> weather { get; set; }
        public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        public Sys sys { get; set; }
        public DateTime dt_txt { get; set; }
        public Rain rain { get; set; }
        public Snow snow { get; set; }
    }

    public class ForecastQuery
    {
        public int cod { get; set; }
        public double message { get; set; }
        public int cnt { get; set; }
        public List<ForecastList> list { get; set; }
        public City city { get; set; }
    }

    public class ForecastItem
    {
        public bool ValidRequest { get; set; }
        public DateTime DataTimestamp { get; set; }
        public string Town { get; set; }

        public int cod { get; set; }
        public double message { get; set; }
        public int cnt { get; set; }
        public List<ForecastList> list { get; set; }
        public City city { get; set; }
        
        [JsonIgnore]
        TemperatureObj[] temp_min_max_5d;
        [JsonIgnore]
        public TemperatureObj[] temp_min_max_5days
        {
            get
            {
                if (temp_min_max_5d == null)
                {
                    temp_min_max_5d = new TemperatureObj[5];
                    DateTime date = list[0].dt_txt;
                    for (int c = 0; c < 5; c++)
                    {
                        if (c > 0)
                            date = date.AddDays(1);
                        var forecasts = list.Where(f => f.dt_txt.Year == date.Year && f.dt_txt.Month == date.Month && f.dt_txt.Day == date.Day).ToList();
                        temp_min_max_5d[c] = new TemperatureObj(0, forecasts.Min(f => f.main.temp_min), forecasts.Max(f => f.main.temp_max));
                    }
                }
                return temp_min_max_5d;
            }
        }

        public ForecastItem() { }

        public ForecastItem(ForecastQuery query, string town)
        {
            cod = query.cod;
            message = query.message;
            cnt = query.cnt;
            list = query.list;
            city = query.city;
            DataTimestamp = DateTime.Now;
            ValidRequest = cod == 200;
            Town = town;
        }
    }
}
