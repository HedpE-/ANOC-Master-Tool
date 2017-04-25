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
			
			using (FileStream fs = file.OpenRead())
				using (StreamReader sr = new StreamReader(fs))
					using (JsonTextReader reader = new JsonTextReader(sr))
			{
				while (reader.Read())
				{
					var t = reader.Value != null ? reader.Value.ToString() : string.Empty;
					var t2 = reader.ArrayPool;
					var t3 = reader.Value;
					var t4 = reader.TokenType;
					if (reader.TokenType == JsonToken.StartObject)
					{
						// Load each object from the stream and do something with it
						JObject obj = JObject.Load(reader);
						var jSon = obj.ToObject<City>();
						if(jSon.country == "GB")
							Add(jSon);
					}
				}
			}
		}
	}
}
