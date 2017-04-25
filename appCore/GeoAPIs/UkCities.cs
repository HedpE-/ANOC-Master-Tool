/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 24/04/2017
 * Time: 04:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using OpenWeatherAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace appCore.GeoAPIs
{
	/// <summary>
	/// Description of UkCities.
	/// </summary>
	public class UkCities : List<City>
	{
		public UkCities()
		{
			FileInfo file = new FileInfo(Settings.GlobalProperties.DBFilesDefaultLocation.FullName + @"\city.list.json");
			using(var jsonReader = new JsonTextReader(file.OpenText())){
				while(jsonReader.Read()){
					var t = jsonReader.Value != null ? jsonReader.Value.ToString() : string.Empty;
					var t2 = jsonReader.ArrayPool;
					var t3 = jsonReader.Value;
					//evaluate the current node and whether it's the name you want
//					if(jsonReader.TokenType.ToString() == "GB"){
						//do what you want
						JsonSerializer serializer = new JsonSerializer();

						// read the json from a stream
						// json size doesn't matter because only a small piece is read at a time from the HTTP request
//						var list = serializer.Deserialize<List<City>>(jsonReader);
//						foreach(City city in list)
//							if(city.country == "GB")
//								Add(city);
//					} else {
//						//break out of loop.
//					}
				}
			}
			JObject jsonData = JObject.Parse(File.ReadAllText(Settings.GlobalProperties.DBFilesDefaultLocation.FullName + @"\\city.list.json"));
			var baseStr = jsonData.SelectToken("base").ToString();
//			using (WebClient client = new WebClient())
//			{
			using (StreamReader sr = File.OpenText(Settings.GlobalProperties.DBFilesDefaultLocation.FullName + @"\\city.list.json"))
			{
				using (JsonReader reader = new JsonTextReader(sr))
				{
					JsonSerializer serializer = new JsonSerializer();

					// read the json from a stream
					// json size doesn't matter because only a small piece is read at a time from the HTTP request
					var list = serializer.Deserialize<List<City>>(reader);
					foreach(City city in list)
						if(city.country == "GB")
							Add(city);
//						AddRange(serializer.Deserialize<List<City>>(reader));
				}
			}
//			}
		}
	}
}
