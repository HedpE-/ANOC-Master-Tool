/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 29/05/2017
 * Time: 05:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenWeatherAPI;

namespace appCore.DB
{
	/// <summary>
	/// Description of WeatherCollection.
	/// </summary>
	public static class WeatherCollection
	{
		static FileInfo DbFile;
		
		public static void Initialize(string filePath) {
			DbFile = new FileInfo(filePath);
			
			if(!DbFile.Exists) {
				DbFile.Create();
				DbFile = new FileInfo(DbFile.FullName);
			}
		}
		
		public static void AddWeatherData(Query json) {
			List<Query> list = JsonConvert.DeserializeObject<List<Query>>(File.ReadAllText(DbFile.FullName)) ?? new List<Query>();
			list.Add(json);
			
			using (StreamWriter sw = DbFile.CreateText())
				sw.Write(JsonConvert.SerializeObject(list, Formatting.Indented).ToLower());
		}
		
		public static Query RetrieveExistingWeatherData(string Town) {
			List<Query> list = new List<Query>();
			using (FileStream fs = DbFile.OpenRead())
				using (StreamReader sr = new StreamReader(fs))
					using (JsonTextReader reader = new JsonTextReader(sr))
			{
				while (reader.Read())
				{
					if (reader.TokenType == JsonToken.StartObject)
					{
						// Load each object from the stream and do something with it
						JObject obj = JObject.Load(reader);
						Query query = new Query(obj);
						
						list.Add(query);
					}
				}
			}
			
			
//			List<Query> list = JsonConvert.DeserializeObject<List<Query>>(File.ReadAllText(DbFile.FullName)) ?? new List<Query>();
			
			Query weatherData = list.FirstOrDefault(w => w.Name == Town);
			
			if(weatherData != null) {
				if(DateTime.Now - weatherData.LastUpdateTimestamp > new TimeSpan(2, 0, 0)) {
					list.RemoveAt(list.IndexOf(weatherData));
					
					using (StreamWriter sw = DbFile.CreateText())
						sw.Write(JsonConvert.SerializeObject(list, Formatting.Indented));
					
					weatherData = null;
				}
			}
			
			return weatherData;
		}
	}
}
