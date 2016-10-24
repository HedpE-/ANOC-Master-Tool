/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 21-07-2016
 * Time: 19:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace appCore.SiteFinder
{
	/// <summary>
	/// Description of Vendor.
	/// </summary>
	public partial class Site
	{
		public enum Vendors : byte {
			Ericsson,
			ALU,
			Huawei,
			NSN,
			None
		};
	}
}
