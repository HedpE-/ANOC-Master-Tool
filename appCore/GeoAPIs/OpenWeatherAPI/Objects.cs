using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
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
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }

        public class TemperatureObj
        {
            private double current_kel_temp, temp_kel_min, temp_kel_max;
            private double current_cel_temp, temp_cel_min, temp_cel_max;

            public double CelsiusCurrent { get { return current_cel_temp; } }
            public double FahrenheitCurrent { get { return convertToFahrenheit(current_cel_temp); } }
            public double KelvinCurrent { get { return current_kel_temp; } }
            public double CelsiusMinimum { get { return temp_cel_min; } }
            public double CelsiusMaximum { get { return temp_cel_max; } }
            public double FahrenheitMinimum { get { return convertToFahrenheit(temp_cel_min); } }
            public double FahrenheitMaximum { get { return convertToFahrenheit(temp_cel_max); } }
            public double KelvinMinimum { get { return temp_kel_min; } }
            public double KelvinMaximum { get { return temp_kel_max; } }

            public TemperatureObj(double temp, double min, double max)
            {
                current_kel_temp = temp;
                temp_kel_min = min;
                temp_kel_max = max;
                current_cel_temp = convertToCelsius(current_kel_temp);
                temp_cel_min = convertToCelsius(temp_kel_min);
                temp_cel_max = convertToCelsius(temp_kel_max);
            }

            private double convertToFahrenheit(double celsius)
            {
                return Math.Round(((9.0 / 5.0) * celsius) + 32, 3);
            }

            private double convertToCelsius(double kelvin)
            {
                return Math.Round(kelvin - 273.15, 3);
            }
        }
    }

    public class Wind
        {
            public enum DirectionEnum
            {
                North,
                North_North_East,
                North_East,
                East_North_East,
                East,
                East_South_East,
                South_East,
                South_South_East,
                South,
                South_South_West,
                South_West,
                West_South_West,
                West,
                West_North_West,
                North_West,
                North_North_West,
                Unknown
            }

            public double speed { get; set; }
            [JsonIgnore]
            public double SpeedMetersPerSecond { get { return speed; } }
            [JsonIgnore]
            public double SpeedFeetPerSecond { get { return speed * 3.28084; } }
            public int deg { get; set; }
            [JsonIgnore]
            public DirectionEnum Direction
            {
                get
                {
                    try { return assignDirection(deg); }
                    catch { return DirectionEnum.Unknown; }
                }
            }

            public string directionEnumToString(DirectionEnum dir)
            {
                switch (dir)
                {
                    case DirectionEnum.East:
                        return "East";
                    case DirectionEnum.East_North_East:
                        return "East North-East";
                    case DirectionEnum.East_South_East:
                        return "East South-East";
                    case DirectionEnum.North:
                        return "North";
                    case DirectionEnum.North_East:
                        return "North East";
                    case DirectionEnum.North_North_East:
                        return "North North-East";
                    case DirectionEnum.North_North_West:
                        return "North North-West";
                    case DirectionEnum.North_West:
                        return "North West";
                    case DirectionEnum.South:
                        return "South";
                    case DirectionEnum.South_East:
                        return "South East";
                    case DirectionEnum.South_South_East:
                        return "South South-East";
                    case DirectionEnum.South_South_West:
                        return "South South-West";
                    case DirectionEnum.South_West:
                        return "South West";
                    case DirectionEnum.West:
                        return "West";
                    case DirectionEnum.West_North_West:
                        return "West North-West";
                    case DirectionEnum.West_South_West:
                        return "West South-West";
                    case DirectionEnum.Unknown:
                        return "Unknown";
                    default:
                        return "Unknown";
                }
            }

            private DirectionEnum assignDirection(double degree)
            {
                if (fB(degree, 348.75, 360))
                    return DirectionEnum.North;
                if (fB(degree, 0, 11.25))
                    return DirectionEnum.North;
                if (fB(degree, 11.25, 33.75))
                    return DirectionEnum.North_North_East;
                if (fB(degree, 33.75, 56.25))
                    return DirectionEnum.North_East;
                if (fB(degree, 56.25, 78.75))
                    return DirectionEnum.East_North_East;
                if (fB(degree, 78.75, 101.25))
                    return DirectionEnum.East;
                if (fB(degree, 101.25, 123.75))
                    return DirectionEnum.East_South_East;
                if (fB(degree, 123.75, 146.25))
                    return DirectionEnum.South_East;
                if (fB(degree, 168.75, 191.25))
                    return DirectionEnum.South;
                if (fB(degree, 191.25, 213.75))
                    return DirectionEnum.South_South_West;
                if (fB(degree, 213.75, 236.25))
                    return DirectionEnum.South_West;
                if (fB(degree, 236.25, 258.75))
                    return DirectionEnum.West_South_West;
                if (fB(degree, 258.75, 281.25))
                    return DirectionEnum.West;
                if (fB(degree, 281.25, 303.75))
                    return DirectionEnum.West_North_West;
                if (fB(degree, 303.75, 326.25))
                    return DirectionEnum.North_West;
                if (fB(degree, 326.25, 348.75))
                    return DirectionEnum.North_North_West;
                return DirectionEnum.Unknown;
            }

            //fB = fallsBetween
            private bool fB(double val, double min, double max)
            {
                if ((min <= val) && (val <= max))
                    return true;
                return false;
            }
        }

    public class Clouds
    {
        public int all { get; set; }
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

    public class WeatherQuery
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

    public class WeatherItem
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

        public WeatherItem() { }

        public WeatherItem(WeatherQuery query, string town)
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
