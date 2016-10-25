/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 28-07-2016
 * Time: 22:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 * http://stackoverflow.com/questions/13675154/how-to-get-cookies-info-inside-of-a-cookiecontainer-all-of-them-not-for-a-spe
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
// TODO: DataTableExtension class
public static class DataTableExtension
{
	public static List<Cookie> ToList(this CookieContainer container)
	{
		if(container == null)
			return new List<Cookie>();
		
		var cookies = new List<Cookie>();

		var table = (Hashtable)container.GetType().InvokeMember("m_domainTable",
		                                                        BindingFlags.NonPublic |
		                                                        BindingFlags.GetField |
		                                                        BindingFlags.Instance,
		                                                        null,
		                                                        container,
		                                                        new object[] { });

		foreach (var key in table.Keys)
		{

			Uri uri = null;

			var domain = key as string;

			if (domain == null)
				continue;

			if (domain.StartsWith("."))
				domain = domain.Substring(1);

			var address = string.Format("http://{0}/", domain);

			if (Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out uri) == false)
				continue;

			foreach (Cookie cookie in container.GetCookies(uri))
			{
				cookies.Add(cookie);
			}

		}
		return cookies;
	}
	
	public static CookieContainer ToCookieContainer(this List<Cookie> list)
	{
		var container = new CookieContainer();
		
		if(list == null)
			return container;
		if(list.Count == 0)
			return container;

		foreach (Cookie c in list)
			container.Add(c);
		
		return container;
	}
}