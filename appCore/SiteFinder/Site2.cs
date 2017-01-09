/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 09-01-2017
 * Time: 22:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using FileHelpers;

namespace appCore.SiteFinder
{
	/// <summary>
	/// Description of Site2.
	/// </summary>
	[DelimitedRecord(",")]
	public class Site2
	{
		[FieldNullValue(typeof (string), "")]
		public string SITE;
		[FieldNullValue(typeof (string), "")]
		public string JVCO_ID;
		[FieldNullValue(typeof (string), "")]
		public string GSM900;
		[FieldNullValue(typeof (string), "")]
		public string GSM1800;
		[FieldNullValue(typeof (string), "")]
		public string UMTS900;
		[FieldNullValue(typeof (string), "")]
		public string UMTS2100;
		[FieldNullValue(typeof (string), "")]
		public string LTE800;
		[FieldNullValue(typeof (string), "")]
		public string LTE2600;
		[FieldNullValue(typeof (string), "")]
		public string LTE2100;
//		[FieldConverter(ConverterKind.Double, "000000")]
		[FieldNullValue(typeof (string), "")]
		public string EASTING;
//		[FieldConverter(ConverterKind.Double, "000000")]
		[FieldNullValue(typeof (string), "")]
		public string NORTHING;
		[FieldNullValue(typeof (string), "")]
		public string HOST;
		[FieldNullValue(typeof (string), "")]
		public string PRIORITY;
		[FieldNullValue(typeof (string), "")]
		public string ADDRESS;
		[FieldNullValue(typeof (string), "")]
		public string TELLABSATRISK;
		[FieldNullValue(typeof (string), "")]
		public string AREA;
		[FieldNullValue(typeof (string), "")]
		public string NSN_STATUS;
		[FieldNullValue(typeof (string), "")]
		public string NOC2G;
		[FieldNullValue(typeof (string), "")]
		public string NOC3G;
		[FieldNullValue(typeof (string), "")]
		public string NOC4G;
		[FieldNullValue(typeof (string), "")]
		public string VF_REGION;
		[FieldNullValue(typeof (string), "")]
		public string SPECIAL_ID;
		[FieldNullValue(typeof (string), "")]
		public string SPECIAL;
//		[FieldConverter(ConverterKind.Date, "dd-MM-yyyy")]
//		[FieldNullValue(typeof (DateTime), "01-01-1900")]
		[FieldNullValue(typeof (string), "")]
		public string SPECIAL_START;
//		[FieldConverter(ConverterKind.Date, "dd-MM-yyyy")]
//		[FieldNullValue(typeof (DateTime), "01-01-1900")]
		[FieldNullValue(typeof (string), "")]
		public string SPECIAL_END;
		[FieldNullValue(typeof (string), "")]
		public string VIP;
		[FieldNullValue(typeof (string), "")]
		public string SITE_SHARE_OPERATOR;
		[FieldNullValue(typeof (string), "")]
		public string SITE_SHARE_SITE_NO;
		[FieldNullValue(typeof (string), "")]
		public string SITE_ACCESS;
		[FieldNullValue(typeof (string), "")]
		public string SITE_TYPE;
		[FieldNullValue(typeof (string), "")]
		public string SITE_SUBTYPE;
//		[FieldConverter(ConverterKind.Boolean, "Yes", "No")]
		[FieldNullValue(typeof (string), "")]
		public string PAKNET_FITTED;
//		[FieldConverter(ConverterKind.Boolean, "Yes", "No")]
		[FieldNullValue(typeof (string), "")]
		public string VODAPAGE_FITTED;
		[FieldNullValue(typeof (string), "")]
		public string DC_STATUS;
//		[FieldConverter(ConverterKind.Date, "dd-MMM-yyyy")]
//		[FieldNullValue(typeof (DateTime), "01-01-1900")]
		[FieldNullValue(typeof (string), "")]
		public string DC_TIMESTAMP;
		[FieldNullValue(typeof (string), "")]
		public string COOLING_STATUS;
//		[FieldConverter(ConverterKind.Date, "dd-MMM-yyyy")]
//		[FieldNullValue(typeof (DateTime), "01-01-1900")]
		[FieldNullValue(typeof (string), "")]
		public string COOLING_TIMESTAMP;
		[FieldNullValue(typeof (string), "")]
		public string KEY_INFORMATION;
		[FieldNullValue(typeof (string), "")]
		public string EF_HEALTHANDSAFETY;
		[FieldNullValue(typeof (string), "")]
		public string SWITCH2G;
		[FieldNullValue(typeof (string), "")]
		public string SWITCH3G;
		[FieldNullValue(typeof (string), "")]
		public string SWITCH4G;
		[FieldNullValue(typeof (string), "")]
		public string DRSWITCH2G;
		[FieldNullValue(typeof (string), "")]
		public string DRSWITCH3G;
		[FieldNullValue(typeof (string), "")]
		public string DRSWITCH4G;
		[FieldNullValue(typeof (string), "")]
		public string MTX2G;
		[FieldNullValue(typeof (string), "")]
		public string MTX3G;
		[FieldNullValue(typeof (string), "")]
		public string MTX4G;
		[FieldNullValue(typeof (string), "")]
		public string IP_2G_I;
		[FieldNullValue(typeof (string), "")]
		public string IP_2G_E;
		[FieldNullValue(typeof (string), "")]
		public string IP_3G_I;
		[FieldNullValue(typeof (string), "")]
		public string IP_3G_E;
		[FieldNullValue(typeof (string), "")]
		public string IP_4G_I;
		[FieldNullValue(typeof (string), "")]
		public string IP_4G_E;
		[FieldNullValue(typeof (string), "")]
		public string VENDOR_2G;
		[FieldNullValue(typeof (string), "")]
		public string VENDOR_3G;
		[FieldNullValue(typeof (string), "")]
		public string VENDOR_4G;
//		[FieldConverter(ConverterKind.Date, "dd-MM-yyyy")]
//		[FieldNullValue(typeof (DateTime), "01-01-1900")]
		[FieldNullValue(typeof (string), "")]
		public string DATE;
		[FieldNullValue(typeof (string), "")]
		public string MTX_RELATED;
		
		public Site2()
		{
		}
	}
}
