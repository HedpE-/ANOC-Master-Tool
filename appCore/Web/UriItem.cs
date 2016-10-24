/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 30-07-2016
 * Time: 03:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace appCore.Web
{
	/// <summary>
	/// Description of UriItem.
	/// </summary>
	[Serializable]
	public class UriItem {
		public string Name { get; set; }
		public Uri URI { get; set; }
		
		public UriItem(string name, Uri uri)
		{
			Name = name;
			URI = uri;
		}
	}
}