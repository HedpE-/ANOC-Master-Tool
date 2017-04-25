/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 24/04/2017
 * Time: 14:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OpenWeatherAPI
{
	public class City
	{
		public int id { get; set; }
		public string name { get; set; }
		public string country { get; set; }
		public Coord coord { get; set; }

        public City(JToken coordData)
        {
        	id = int.Parse(coordData.SelectToken("id").ToString());
            name = coordData.SelectToken("name").ToString();
            country = coordData.SelectToken("country").ToString();
            coord = new Coord(coordData.SelectToken("coord"));
        }
        
        public City()
        {
        }
	}
}
