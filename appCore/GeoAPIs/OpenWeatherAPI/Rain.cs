using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenWeatherAPI
{
    public class Rain
    {
        [JsonProperty(PropertyName = "3h")]
        public double h3;

        public Rain() { }

            public Rain(JToken rainData)
        {
            if (rainData.SelectToken("3h") != null)
                h3 = double.Parse(rainData.SelectToken("3h").ToString());
        }
    }
}
