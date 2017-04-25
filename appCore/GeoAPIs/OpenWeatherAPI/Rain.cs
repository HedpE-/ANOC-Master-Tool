using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OpenWeatherAPI
{
    public class Rain
    {
        private double h3;

        public double H3 { get { return h3; } }

        public Rain(JToken rainData)
        {
            if (rainData.SelectToken("3h") != null)
                h3 = double.Parse(rainData.SelectToken("3h").ToString());
        }
    }
}
