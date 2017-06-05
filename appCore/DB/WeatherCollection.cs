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
using System.Threading;
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
			
			while(!DbFile.Exists)
            {
                try
                {
                    DbFile.Create();
                    DbFile = new FileInfo(DbFile.FullName);
                }
                catch(Exception e) // if creation fails probably the file already exists
                {
                    DbFile = new FileInfo(DbFile.FullName);
                }
            }

            if(Settings.CurrentUser.UserName == "GONCARJ3")
            {
                Thread thread = new Thread(() =>
                {
                    List<WeatherItem> list = JsonConvert.DeserializeObject<List<WeatherItem>>(ReadAllTextFromDbFile()) ?? new List<WeatherItem>();

                    for (int c = 0; c < list.Count; c++)
                    {
                        if (DateTime.Now - list[c].DataTimestamp > new TimeSpan(2, 0, 0))
                            list.RemoveAt(c--);
                    }

                    SerializeListToDbFile(list);
                })
                {
                    Name = "Databases.WeatherCollection.Initialize_RemoveExpiredWeatherItems"
                };
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        public static WeatherItem RetrieveWeatherData(string Town)
        {
            WeatherItem weatherItem = RetrieveExistingWeatherData(Town);
            if (weatherItem == null)
            {
                OpenWeatherAPI.OpenWeatherAPI openWeatherAPI = new OpenWeatherAPI.OpenWeatherAPI("7449082d365b8a6314614efed99d2696");
                weatherItem = new WeatherItem(openWeatherAPI.queryCityName(Town + ",UK"), Town);
                AddWeatherData(weatherItem);
            }

            return weatherItem;
        }

        public static List<WeatherItem> RetrieveWeatherData(List<int> ids)
        {
            var tempIds = ids;
            List<WeatherItem> foundItems = RetrieveExistingWeatherData(ref tempIds).ToList();

            if (tempIds.Any())
            {
                OpenWeatherAPI.OpenWeatherAPI openWeatherAPI = new OpenWeatherAPI.OpenWeatherAPI("7449082d365b8a6314614efed99d2696");
                int c = 0;
                while (c < tempIds.Count)
                {
                    List<int> tempArr = new List<int>();
                    for (c = c;c < tempArr.Count && ((c + 1) % 20 > 0); c++)
                        tempArr.Add(ids[c]);

                    var query = openWeatherAPI.queryBulkCityId(tempArr);
                    foundItems.AddRange(query);
                    AddWeatherData(query);
                }
            }

            return foundItems;
        }

        static void AddWeatherData(WeatherItem json)
        {

            List<WeatherItem> list = new List<WeatherItem>();
            list.Add(json);

            AddWeatherData(list);
        }

        static void AddWeatherData(List<WeatherItem> weatherCollection)
        {
            List<WeatherItem> list = JsonConvert.DeserializeObject<List<WeatherItem>>(ReadAllTextFromDbFile()) ?? new List<WeatherItem>();
            list.AddRange(weatherCollection);

            SerializeListToDbFile(list);
        }

        static WeatherItem RetrieveExistingWeatherData(string Town)
        {
            List<WeatherItem> list = JsonConvert.DeserializeObject<List<WeatherItem>>(ReadAllTextFromDbFile()) ?? new List<WeatherItem>();

            WeatherItem weatherItem = list.FirstOrDefault(w => String.Equals(w.Town, Town, StringComparison.OrdinalIgnoreCase));
			
			if(weatherItem != null) {
                if (DateTime.Now - weatherItem.DataTimestamp > new TimeSpan(2, 0, 0))
                {
                    list.RemoveAt(list.IndexOf(weatherItem));

                    SerializeListToDbFile(list);

                    weatherItem = null;
                }
            }
			
			return weatherItem;
        }

        static List<WeatherItem> RetrieveExistingWeatherData(ref List<int> ids)
        {
            List<WeatherItem> list = JsonConvert.DeserializeObject<List<WeatherItem>>(ReadAllTextFromDbFile()) ?? new List<WeatherItem>();

            List<int> tempList = ids;
            List<WeatherItem> foundItems = new List<WeatherItem>();
            for(int c = 0;c < tempList.Count;c++)
            {
                City city = Databases.Cities.FirstOrDefault(ct => ct.id == tempList[c]);
                WeatherItem weatherItem = RetrieveExistingWeatherData(city.name);
                if (weatherItem != null)
                {
                    foundItems.Add(weatherItem);
                    tempList.RemoveAt(c--);
                }
            }

            ids = tempList;

            return foundItems;
        }

        static string ReadAllTextFromDbFile()
        {
            bool operationFinished = false;
            string contents = string.Empty;
            while (!operationFinished)
            {
                try
                {
                    contents = File.ReadAllText(DbFile.FullName);
                    operationFinished = true;
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }

            return contents;
        }

        static void SerializeListToDbFile(List<WeatherItem> list)
        {
            bool operationFinished = false;
            while (!operationFinished)
            {
                try
                {
                    using (StreamWriter sw = DbFile.CreateText())
                    {
                        sw.Write(JsonConvert.SerializeObject(list, Formatting.Indented));
                        sw.Close();
                    }

                    operationFinished = true;
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}
