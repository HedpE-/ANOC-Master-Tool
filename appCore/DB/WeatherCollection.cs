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
using appCore.Settings;
using OpenWeatherAPI;
using OpenWeatherAPI.CurrentWeather;
using OpenWeatherAPI.Forecast;

namespace appCore.DB
{
	/// <summary>
	/// Description of WeatherCollection.
	/// </summary>
	public static class WeatherCollection
	{
		static FileInfo CurrentWeatherDbFile;
        static FileInfo ForecastDbFile;

        public static void Initialize(string DbFilesDir) {
            CurrentWeatherDbFile = new FileInfo(DbFilesDir + @"\CurrentWeatherDB.json");
            ForecastDbFile = new FileInfo(DbFilesDir + @"\ForecastWeatherDB.json");

            List<Thread> threads = new List<Thread>();
            int finishedThreadsCount = 0;
            Thread thread = new Thread(() => {
                while (!CurrentWeatherDbFile.Exists)
                {
                    try
                    {
                        using (CurrentWeatherDbFile.Create()) { };
                        CurrentWeatherDbFile = new FileInfo(CurrentWeatherDbFile.FullName);
                    }
                    catch (Exception e) // if creation fails probably the file already exists
                    {
                        CurrentWeatherDbFile = new FileInfo(CurrentWeatherDbFile.FullName);
                    }
                }
                finishedThreadsCount++;
            });
            thread.Name = "CurrentWeatherDbFile.Create()";
            threads.Add(thread);

            thread = new Thread(() => {
                while (!ForecastDbFile.Exists)
                {
                    try
                    {
                        using (ForecastDbFile.Create()) { };
                        ForecastDbFile = new FileInfo(ForecastDbFile.FullName);
                    }
                    catch (Exception e) // if creation fails probably the file already exists
                    {
                        ForecastDbFile = new FileInfo(ForecastDbFile.FullName);
                    }
                }
                finishedThreadsCount++;
            });
            thread.Name = "ForecastDbFile.Create()";
            threads.Add(thread);

            foreach (Thread th in threads)
            {
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
            }

            while (finishedThreadsCount < threads.Count) { }

            if (CurrentUser.UserName == "GONCARJ3")
            {
                threads.Clear();
                thread = new Thread(() =>
                {
                    List<CurrentWeatherItem> list = JsonConvert.DeserializeObject<List<CurrentWeatherItem>>(ReadAllTextFromDbFile("CurrentWeather")) ?? new List<CurrentWeatherItem>();

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
                threads.Add(thread);

                thread = new Thread(() =>
                {
                    List<ForecastItem> list = JsonConvert.DeserializeObject<List<ForecastItem>>(ReadAllTextFromDbFile("Forecast")) ?? new List<ForecastItem>();

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
                threads.Add(thread);

                foreach (Thread th in threads)
                {
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();
                }

                //while (finishedThreadsCount < threads.Count) { }
            }
        }

        public static WeatherItem RetrieveWeatherData(string Town)
        {
            WeatherItem weatherItem = RetrieveExistingWeatherData(Town);
            if (weatherItem.CurrentWeather == null || weatherItem.Forecast5D3H == null)
            {
                OpenWeatherAPI.OpenWeatherAPI openWeatherAPI = new OpenWeatherAPI.OpenWeatherAPI("7449082d365b8a6314614efed99d2696");

                if (weatherItem.CurrentWeather == null)
                {
                    weatherItem.CurrentWeather = new CurrentWeatherItem(openWeatherAPI.queryCurrentWeatherCityName(Town + ",UK"), Town);
                    AddWeatherData(weatherItem.CurrentWeather);
                }

                if (weatherItem.Forecast5D3H == null)
                {
                    weatherItem.Forecast5D3H = new ForecastItem(openWeatherAPI.queryForecastCityName(Town + ",UK"), Town);
                    AddWeatherData(weatherItem.Forecast5D3H);
                }
            }

            return weatherItem;
        }

        //public static List<CurrentWeatherItem> RetrieveWeatherData(List<int> ids)
        //{
        //    var tempIds = ids;
        //    List<CurrentWeatherItem> foundItems = RetrieveExistingWeatherData(ref tempIds).ToList();

        //    if (tempIds.Any())
        //    {
        //        OpenWeatherAPI.OpenWeatherAPI openWeatherAPI = new OpenWeatherAPI.OpenWeatherAPI("7449082d365b8a6314614efed99d2696");
        //        int c = 0;
        //        while (c < tempIds.Count)
        //        {
        //            List<int> tempArr = new List<int>();
        //            for (c = c;c < tempArr.Count && ((c + 1) % 20 > 0); c++)
        //                tempArr.Add(ids[c]);

        //            var query = openWeatherAPI.queryCurrentWeatherBulkCityId(tempArr);
        //            foundItems.AddRange(query);
        //            AddWeatherData(query);
        //        }
        //    }

        //    return foundItems;
        //}

        static void AddWeatherData(CurrentWeatherItem json)
        {

            List<CurrentWeatherItem> list = new List<CurrentWeatherItem>();
            list.Add(json);

            AddWeatherData(list);
        }

        static void AddWeatherData(List<CurrentWeatherItem> weatherCollection)
        {
            List<CurrentWeatherItem> list = JsonConvert.DeserializeObject<List<CurrentWeatherItem>>(ReadAllTextFromDbFile("CurrentWeather")) ?? new List<CurrentWeatherItem>();
            list.AddRange(weatherCollection);

            SerializeListToDbFile(list);
        }

        static void AddWeatherData(ForecastItem json)
        {
            List<ForecastItem> list = new List<ForecastItem>();
            list.Add(json);

            AddWeatherData(list);
        }

        static void AddWeatherData(List<ForecastItem> weatherCollection)
        {
            List<ForecastItem> list = JsonConvert.DeserializeObject<List<ForecastItem>>(ReadAllTextFromDbFile("Forecast")) ?? new List<ForecastItem>();
            list.AddRange(weatherCollection);

            SerializeListToDbFile(list);
        }

        static WeatherItem RetrieveExistingWeatherData(string Town)
        {
            WeatherItem weatherItem = new WeatherItem()
            {
                CurrentWeather = RetrieveExistingCurrentWeatherData(Town),
                Forecast5D3H = RetrieveExistingForecastWeatherData(Town)
            };

            return weatherItem;
        }

        //static List<WeatherItem> RetrieveExistingWeatherData(ref List<int> ids)
        //{
        //    List<CurrentWeatherItem> list = JsonConvert.DeserializeObject<List<CurrentWeatherItem>>(ReadAllTextFromDbFile("CurrentWeather")) ?? new List<CurrentWeatherItem>();

        //    List<int> tempList = ids;
        //    List<CurrentWeatherItem> foundItems = new List<CurrentWeatherItem>();
        //    for(int c = 0;c < tempList.Count;c++)
        //    {
        //        City city = Databases.Cities.FirstOrDefault(ct => ct.id == tempList[c]);
        //        WeatherItem weatherItem = RetrieveExistingWeatherData(city.name);
        //        if (weatherItem != null)
        //        {
        //            foundItems.Add(weatherItem);
        //            tempList.RemoveAt(c--);
        //        }
        //    }

        //    ids = tempList;

        //    return foundItems;
        //}

        static CurrentWeatherItem RetrieveExistingCurrentWeatherData(string Town)
        {
            List<CurrentWeatherItem> list = JsonConvert.DeserializeObject<List<CurrentWeatherItem>>(ReadAllTextFromDbFile("CurrentWeather")) ?? new List<CurrentWeatherItem>();

            CurrentWeatherItem weatherItem = list.FirstOrDefault(w => String.Equals(w.Town, Town, StringComparison.OrdinalIgnoreCase));

            if (weatherItem != null)
            {
                if (DateTime.Now - weatherItem.DataTimestamp > new TimeSpan(2, 0, 0))
                {
                    list.RemoveAt(list.IndexOf(weatherItem));

                    SerializeListToDbFile(list);

                    weatherItem = null;
                }
            }

            return weatherItem;
        }

        static ForecastItem RetrieveExistingForecastWeatherData(string Town)
        {
            List<ForecastItem> list = JsonConvert.DeserializeObject<List<ForecastItem>>(ReadAllTextFromDbFile("Forecast")) ?? new List<ForecastItem>();

            ForecastItem weatherItem = list.FirstOrDefault(w => String.Equals(w.Town, Town, StringComparison.OrdinalIgnoreCase));

            if (weatherItem != null)
            {
                if (DateTime.Now.Day != weatherItem.list[0].dt_txt.Day || DateTime.Now.Month != weatherItem.list[0].dt_txt.Month || DateTime.Now.Year != weatherItem.list[0].dt_txt.Year)
                {
                    list.RemoveAt(list.IndexOf(weatherItem));

                    SerializeListToDbFile(list);

                    weatherItem = null;
                }
            }

            return weatherItem;
        }

        static string ReadAllTextFromDbFile(string queryType)
        {
            bool operationFinished = false;
            string contents = string.Empty;
            while (!operationFinished)
            {
                try
                {
                    contents = File.ReadAllText(queryType == "CurrentWeather" ? CurrentWeatherDbFile.FullName : ForecastDbFile.FullName);
                    operationFinished = true;
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }

            return contents;
        }

        static void SerializeListToDbFile(List<CurrentWeatherItem> list)
        {
            bool operationFinished = false;
            while (!operationFinished)
            {
                try
                {
                    using (StreamWriter sw = CurrentWeatherDbFile.CreateText())
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

        static void SerializeListToDbFile(List<ForecastItem> list)
        {
            bool operationFinished = false;
            while (!operationFinished)
            {
                try
                {
                    using (StreamWriter sw = ForecastDbFile.CreateText())
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
