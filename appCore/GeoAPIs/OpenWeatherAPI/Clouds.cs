using System;
using Newtonsoft.Json;

namespace OpenWeatherAPI
{
    public class Clouds
    {
        public int all { get; set; }
        [JsonIgnore]
        public System.Drawing.Image Picture = System.Drawing.Image.FromFile(appCore.Settings.GlobalProperties.WeatherPicturesLocation.FullName + @"\cloudiness.png");
    }
}
