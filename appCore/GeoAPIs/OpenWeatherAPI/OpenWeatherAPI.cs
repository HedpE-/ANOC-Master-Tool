using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
		public Query query(string queryStr)
		{
			JObject jsonData = JObject.Parse(new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/weather?appid={0}&q={1}", openWeatherAPIKey, queryStr)));
			Query newQuery = new Query(jsonData, queryStr);
			
			return newQuery.ValidRequest ? newQuery : null;
		}

		//Returns null if invalid request
		public Query query(double latitude, double longitude, string cityName)
		{
			JObject jsonData = JObject.Parse(new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/weather?appid={0}&lat={1}&lon={2}", openWeatherAPIKey, latitude, longitude)));
			Query newQuery = new Query(jsonData, cityName);
			
			return newQuery.ValidRequest ? newQuery : null;
        }

        //Returns null if invalid request
        public List<Query> query(IEnumerable<string> cities)
        {
            string cityIds = string.Empty;
            foreach(string city in cities) {
                var foundCity = appCore.DB.Databases.Cities.FirstOrDefault(c => c.name.ToUpper() == city);
                if (foundCity != null)
                    cityIds += foundCity.id + ",";
            }
            if (cityIds.EndsWith(","))
                cityIds = cityIds.Substring(0, cityIds.Length - 2);

            if (!string.IsNullOrEmpty(cityIds))
            {
                JObject jsonData = JObject.Parse(new System.Net.WebClient().DownloadString(string.Format("http://api.openweathermap.org/data/2.5/group?appid={0}&id={1}", openWeatherAPIKey, cityIds)));
                List<Query> bulkQuery = new List<Query>();
                //foreach(Query(jsonData);

                return bulkQuery;
            }

            return null;
        }
    }
}
