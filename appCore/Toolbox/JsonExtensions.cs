/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 31/05/2017
 * Time: 05:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public static class JsonExtensions
{
	public static JToken SelectToken(this JObject jObject, string name, bool errorWhenNoMatch, bool caseSensitive) {
		JToken token = null;
		token = caseSensitive ?
			jObject.SelectToken(name, errorWhenNoMatch) :
			jObject.Descendants().FirstOrDefault(t => String.Equals(t.Path, name, StringComparison.OrdinalIgnoreCase));
		
		return token;
	}
	
	public static JToken SelectToken(this JToken jToken, string name, bool errorWhenNoMatch, bool caseSensitive) {
		JToken token = null;
		string str = string.Empty;
		foreach(var test in jToken.Children())
			str = test.Path;
		
		token = caseSensitive ?
			jToken.SelectToken(name, errorWhenNoMatch) :
			jToken.Children().FirstOrDefault(t => String.Equals(t.Path, name, StringComparison.OrdinalIgnoreCase));
		
		return token;
	}
	
//	public static IEnumerable<JToken> CaseSelectPropertyValues(this JToken token, string name)
//	{
//		var obj = token as JObject;
//		if (obj == null)
//			yield break;
//		foreach (var property in obj.Properties())
//		{
//			if (name == null)
//				yield return property.Value;
//			else if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
//				yield return property.Value;
//		}
//	}
//
//	public static IEnumerable<JToken> CaseSelectPropertyValues(this IEnumerable<JToken> tokens, string name)
//	{
//		if (tokens == null)
//			throw new ArgumentNullException();
//		return tokens.SelectMany(t => t.CaseSelectPropertyValues(name));
//	}
}
