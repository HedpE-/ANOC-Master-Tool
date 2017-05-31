/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 30/05/2017
 * Time: 06:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using OpenWeatherAPI;

namespace appCore.Toolbox
{
	public class FactoryConverter<T> : JsonConverter
	{
		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException("CustomCreationConverter should only be used while deserializing.");
		}

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>The object value.</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
//			if (reader.TokenType == JsonToken.Null)
				return null;

//			T value = CreateAndPopulate(objectType, serializer.Deserialize<Dictionary<String, String>>(reader));

//			if (value == null)
//				throw new JsonSerializationException("No object created.");
//
//			return value;
		}

		/// <summary>
		/// Creates an object which will then be populated by the serializer.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns></returns>
//		public abstract T CreateAndPopulate(Type objectType, Dictionary<String, String> jsonFields);

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		///     <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.
		/// </summary>
		/// <value>
		///     <c>true</c> if this <see cref="JsonConverter"/> can write JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}
	}
	
//		public static object Create(Type objectType, JObject jObject)
//		{
//			switch(objectType.Name) {
//				case "OpenWeatherAPI.Coord":
//					return new Coord(jObject);
//				case "OpenWeatherAPI.Main":
//					return new Main(jObject);
//				case "OpenWeatherAPI.Wind":
//					return new Wind(jObject);
//				case "OpenWeatherAPI.Rain":
//					return new Rain(jObject);
//				case "OpenWeatherAPI.Snow":
//					return new Snow(jObject);
//				case "OpenWeatherAPI.Clouds":
//					return new Clouds(jObject);
//				case "OpenWeatherAPI.Sys":
//					return new Sys(jObject);
	////				case typeof(Weather).Name:
	////					return new Coord(Weather).Name;
//			}
//
//			throw new ApplicationException(String.Format("The given objectType {0} is not supported!", objectType));
//		}
}