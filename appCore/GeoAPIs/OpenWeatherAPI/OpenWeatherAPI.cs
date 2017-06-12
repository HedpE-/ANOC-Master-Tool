using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenWeatherAPI.CurrentWeather;
using OpenWeatherAPI.Forecast;

namespace OpenWeatherAPI
{
	public class OpenWeatherAPI
	{
		string openWeatherAPIKey;

		public OpenWeatherAPI(string apiKey)
		{
			openWeatherAPIKey = apiKey;
		}

		public void updateAPIKey(string apiKey)
		{
			openWeatherAPIKey = apiKey;
		}

		//Returns null if invalid request
		public CurrentWeatherQuery queryCurrentWeatherCityName(string queryStr)
		{
            string jSon = new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/weather?appid={0}&q={1}", openWeatherAPIKey, queryStr));
            var query = JsonConvert.DeserializeObject<CurrentWeatherQuery>(jSon);

            return query;
        }

        //Returns null if invalid request
        public List<CurrentWeatherItem> queryCurrentWeatherBulkCityId(List<int> ids)
        {
            string units = "metric";
            string jSon = new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/group?appid={0}&id={1}&units={3}", openWeatherAPIKey, string.Join(",", ids), units));
            var query = JsonConvert.DeserializeObject<List<CurrentWeatherQuery>>(jSon);

            return query.ConvertToWeatherItems();
        }

        //Returns null if invalid request
        public ForecastQuery queryForecastCityName(string queryStr)
        {
            string jSon = new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/forecast?appid={0}&q={1}", openWeatherAPIKey, queryStr));
            var query = JsonConvert.DeserializeObject<ForecastQuery>(jSon);

            return query;
        }

        //Returns null if invalid request
        //public Query queryCityId(int cityId)
        //{
        //	JObject jsonData = JObject.Parse(new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/weather?appid={0}&id={1}", openWeatherAPIKey, cityId)));
        //	Query newQuery = new Query(jsonData);

        //	return newQuery.ValidRequest ? newQuery : null;
        //      }

        //      //Returns null if invalid request
        //      public Query queryZipCode(string postCode)
        //      {
        //          JObject jsonData = JObject.Parse(new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/weather?appid={0}&zip={1},GB", openWeatherAPIKey, postCode)));
        //          Query newQuery = new Query(jsonData, "");

        //          return newQuery.ValidRequest ? newQuery : null;
        //      }

        //      //Returns null if invalid request
        //      public Query queryLongitudeLatitude(double latitude, double longitude, string cityName)
        //{
        //	JObject jsonData = JObject.Parse(new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/weather?appid={0}&lat={1}&lon={2}", openWeatherAPIKey, latitude, longitude)));
        //	Query newQuery = new Query(jsonData, cityName);

        //	return newQuery.ValidRequest ? newQuery : null;
        //      }

        //      //Returns null if invalid request
        //      public List<Query> query(IEnumerable<string> cities)
        //      {
        //          string cityIds = string.Empty;
        //          List<string> citiesNotFound = new List<string>();
        //          foreach(string city in cities) {
        //          	var foundCity = appCore.DB.Databases.Cities.FirstOrDefault(c => c.name.ToUpper() == city.ToUpper());
        //              if(foundCity != null)
        //                  cityIds += foundCity.id + ",";
        //              else
        //              	citiesNotFound.Add(city);
        //          }
        //          if (cityIds.EndsWith(","))
        //              cityIds = cityIds.Substring(0, cityIds.Length - 2);

        //          if (!string.IsNullOrEmpty(cityIds))
        //          {
        //              JObject jsonData = JObject.Parse(new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/group?appid={0}&id={1}", openWeatherAPIKey, cityIds)));
        //              List<Query> bulkQuery = new List<Query>();
        //              //foreach(Query(jsonData);

        //              return bulkQuery;
        //          }

        //          return null;
        //      }

        //      //Returns null if invalid request
        //      public List<Query> query(double minLongitude, double maxLongitude, double minLatitude, double maxLatitude)
        //      {
        //          string req = string.Format("http://api.openweathermap.org/data/2.5/box/city?appid={0}&bbox={1},{2},{3},{4},{5}", openWeatherAPIKey, minLongitude, minLatitude, maxLongitude, maxLatitude, "0");
        //          string resp = new System.Net.WebClient().DownloadString(req);
        //          JObject jsonData = JObject.Parse(resp);
        //              List<Query> bulkQuery = new List<Query>();
        //              //foreach(Query(jsonData);

        //              return bulkQuery;

        //          //return null;
        //      }
    }
}
