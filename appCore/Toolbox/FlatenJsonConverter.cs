/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 30/05/2017
 * Time: 06:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenWeatherAPI;

namespace appCore.Toolbox
{
	/// <summary>
	/// Description of FlatenJsonConverter.
	/// </summary>
	public class FlattenJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value,
		                               JsonSerializer serializer)
		{
			JToken t = JToken.FromObject(value);
			if (t.Type != JTokenType.Object)
			{
				t.WriteTo(writer);
				return;
			}

			JObject o = (JObject)t;
			writer.WriteStartObject();
			WriteJson(writer, o);
			writer.WriteEndObject();
		}

		private void WriteJson(JsonWriter writer, JObject value)
		{
			foreach (var p in value.Properties())
			{
				if (p.Value is JObject)
					WriteJson(writer, (JObject)p.Value);
				else
					p.WriteTo(writer);
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType,
		                                object existingValue, JsonSerializer serializer)
		{
			
			if (reader.TokenType == JsonToken.Null)
				return null;
			// Load JObject from stream
			JObject jObject = JObject.Load(reader);
			// Create target object based on JObject
			object target = Create(objectType, jObject);
			// Populate the object properties
			using (JsonReader jObjectReader = CopyReaderForObject(reader, jObject))
			{
				serializer.Populate(jObjectReader, target);
			}
			return target;
		}
		
		public object Create(Type objectType, JObject jObject)
		{
			switch(objectType.Name) {
				case typeof(Coord).Name:
					return new Coord(jObject);
				case typeof(Main).Name:
					return new Main(jObject);
				case typeof(Wind).Name:
					return new Wind(jObject);
				case typeof(Rain).Name:
					return new Rain(jObject);
				case typeof(Snow).Name:
					return new Snow(jObject);
				case typeof(Clouds).Name:
					return new Clouds(jObject);
				case typeof(Sys).Name:
					return new Sys(jObject);
//				case typeof(Weather).Name:
//					return new Coord(Weather).Name;
			}

			throw new ApplicationException(String.Format("The given objectType {0} is not supported!", objectType));
		}

		public override bool CanConvert(Type objectType)
		{
			return true; // works for any type
		}
		
		/// <summary>Creates a new reader for the specified jObject by copying the settings
		/// from an existing reader.</summary>
		/// <param name="reader">The reader whose settings should be copied.</param>
		/// <param name="jObject">The jObject to create a new reader for.</param>
		/// <returns>The new disposable reader.</returns>
		public static JsonReader CopyReaderForObject(JsonReader reader, JObject jObject)
		{
			JsonReader jObjectReader = jObject.CreateReader();
			jObjectReader.Culture = reader.Culture;
			jObjectReader.DateFormatString = reader.DateFormatString;
			jObjectReader.DateParseHandling = reader.DateParseHandling;
			jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
			jObjectReader.FloatParseHandling = reader.FloatParseHandling;
			jObjectReader.MaxDepth = reader.MaxDepth;
			jObjectReader.SupportMultipleContent = reader.SupportMultipleContent;
			return jObjectReader;
		}
	}
}
