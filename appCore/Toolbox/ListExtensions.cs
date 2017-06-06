/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 06/01/2017
 * Time: 20:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    public static bool Contains(this List<string> list, string pattern, StringComparison stringComparison)
    {
        return list.FindIndex(x => x.Equals(pattern, stringComparison)) != -1;
    }
    
    public static List<OpenWeatherAPI.WeatherItem> ConvertToWeatherItems(this List<OpenWeatherAPI.WeatherQuery> list)
    {
        var newList = list.Select(w => new OpenWeatherAPI.WeatherItem(w, w.name));
        return newList.ToList();
    }

    /// <summary>
    /// Get the array slice between the two indexes.
    /// ... Inclusive for start index, exclusive for end index.
    /// </summary>
    public static T[] Slice<T>(this T[] source, int start, int end)
    {
        // Handles negative ends.
        if (end < 0)
        {
            end = source.Length + end;
        }
        int len = end - start;

        // Return new array.
        T[] res = new T[len];
        for (int i = 0; i < len; i++)
        {
            res[i] = source[i + start];
        }
        return res;
    }
}