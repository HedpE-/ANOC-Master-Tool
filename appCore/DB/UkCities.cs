/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 24/04/2017
 * Time: 04:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using OpenWeatherAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace appCore.DB
{
	/// <summary>
	/// Description of UkCities.
	/// </summary>
	public class UkCities : List<City>
	{
		public UkCities()
		{
			FileInfo file = new FileInfo(Settings.GlobalProperties.DBFilesDefaultLocation.FullName + @"\city.list.json");
			
			using (FileStream fs = file.OpenRead())
				using (StreamReader sr = new StreamReader(fs))
					using (JsonTextReader reader = new JsonTextReader(sr))
			{
				while (reader.Read())
				{
					if (reader.TokenType == JsonToken.StartObject)
					{
						// Load each object from the stream and do something with it
						JObject obj = JObject.Load(reader);
                        City city = new City(obj);
                        
						if(city.country == "GB")
							Add(city);
					}
				}
			}
        }

        public City FindCity(int cityId)
        {
            return this.FirstOrDefault(c => c.id == cityId);
        }

        public City FindCity(string cityName)
        {
            return this.FirstOrDefault(c => c.name == cityName);
        }
    }
}
