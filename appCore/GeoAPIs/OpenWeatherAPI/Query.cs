using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenWeatherAPI
{
	public class Query : appCore.GeoAPIs.WeatherItem
	{
		private bool validRequest;
		private Coord coord;
		private List<Weather> weathers = new List<Weather>();
		private string baseStr;
		private Main main;
		private double visibility;
		private Wind wind;
		private Rain rain;
		private Snow snow;
		private Clouds clouds;
		private Sys sys;
		private int id;
		private string name;
		private int cod;

		public bool ValidRequest { get { return validRequest; } private set { validRequest = value; } }
		public Coord Coord { get { return coord; } private set { coord = value; } }
		public List<Weather> Weathers { get { return weathers; } private set { weathers = value; } }
		public string Base { get { return baseStr; } private set { baseStr = value; } }
		public Main Main { get { return main; } private set { main = value; } }
		public double Visibility { get { return visibility; } private set { visibility = value; } }
		public Wind Wind { get { return wind; } private set { wind = value; } }
		public Rain Rain { get { return rain; } private set { rain = value; } }
		public Snow Snow { get { return snow; } private set { snow = value; } }
		public Clouds Clouds { get { return clouds; } private set { clouds = value; } }
		public Sys Sys { get { return sys; } private set { sys = value; } }
		public int ID { get { return id; } private set { id = value; } }
		public string Name { get { return name; } private set { name = value; } }
		public int Cod { get { return cod; } private set { cod = value; } }
		public string Town { get; private set; }

		public Query(JObject jsonData, string cityStr)
		{
			if(jsonData.SelectToken("cod").ToString() == "200")
			{
				validRequest = true;
				coord = new Coord(jsonData.SelectToken("coord"));
				foreach (JToken weather in jsonData.SelectToken("weather"))
					weathers.Add(new Weather(weather));
				baseStr = jsonData.SelectToken("base").ToString();
				main = new Main(jsonData.SelectToken("main"));
				if(jsonData.SelectToken("visibility") != null)
					visibility = double.Parse(jsonData.SelectToken("visibility").ToString());
				wind = new Wind(jsonData.SelectToken("wind"));
				if(jsonData.SelectToken("raid") != null)
					rain = new Rain(jsonData.SelectToken("rain"));
				if (jsonData.SelectToken("snow") != null)
					snow = new Snow(jsonData.SelectToken("snow"));
				clouds = new Clouds(jsonData.SelectToken("clouds"));
				sys = new Sys(jsonData.SelectToken("sys"));
				id = int.Parse(jsonData.SelectToken("id").ToString());
				name = jsonData.SelectToken("name").ToString();
				cod = int.Parse(jsonData.SelectToken("cod").ToString());
			} else
			{
				validRequest = false;
			}
			Town = cityStr;
		}

		public Query(JObject jsonData)
		{
			if(jsonData.SelectToken("cod").ToString() == "200")
			{
				validRequest = true;
				coord = new Coord(jsonData.SelectToken("coord"));
				foreach (JToken weather in jsonData.SelectToken("weather"))
					weathers.Add(new Weather(weather));
				baseStr = jsonData.SelectToken("base").ToString();
				main = new Main(jsonData.SelectToken("main"));
				if(jsonData.SelectToken("visibility") != null)
					visibility = double.Parse(jsonData.SelectToken("visibility").ToString());
				wind = new Wind(jsonData.SelectToken("wind"));
				if(jsonData.SelectToken("raid") != null)
					rain = new Rain(jsonData.SelectToken("rain"));
				if (jsonData.SelectToken("snow") != null)
					snow = new Snow(jsonData.SelectToken("snow"));
				clouds = new Clouds(jsonData.SelectToken("clouds"));
				sys = new Sys(jsonData.SelectToken("sys"));
				id = int.Parse(jsonData.SelectToken("id").ToString());
				name = jsonData.SelectToken("name").ToString();
				cod = int.Parse(jsonData.SelectToken("cod").ToString());
			} else
			{
				validRequest = false;
			}
		}

//		[JsonConstructor]
		public Query() { }
	}
}
