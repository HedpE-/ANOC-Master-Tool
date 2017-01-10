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
	[DelimitedRecord(","), IgnoreFirst(1)]
	public class Site2
	{
		[FieldOrder(1)]
		[FieldNullValue(typeof (string), "")]
		public string Site;
		[FieldOrder(2)]
		[FieldNullValue(typeof (string), "")]
		public string JVCO_Id;
		[FieldOrder(3)]
		[FieldNullValue(typeof (string), "")]
		public string GSM900;
		[FieldOrder(4)]
		[FieldNullValue(typeof (string), "")]
		public string GSM1800;
		[FieldOrder(5)]
		[FieldNullValue(typeof (string), "")]
		public string UMTS900;
		[FieldOrder(6)]
		[FieldNullValue(typeof (string), "")]
		public string UMTS2100;
		[FieldOrder(7)]
		[FieldNullValue(typeof (string), "")]
		public string LTE800;
		[FieldOrder(8)]
		[FieldNullValue(typeof (string), "")]
		public string LTE2600;
		[FieldOrder(9)]
		[FieldNullValue(typeof (string), "")]
		public string LTE2100;
		[FieldOrder(10)]
//		[FieldConverter(ConverterKind.Double, "000000")]
		[FieldNullValue(typeof (double), "0")]
		public double Easting;
		[FieldOrder(11)]
//		[FieldConverter(ConverterKind.Double, "000000")]
		[FieldNullValue(typeof (double), "0")]
		public double Northing;
		[FieldOrder(12)]
		[FieldNullValue(typeof (string), "")]
		public string Host;
		[FieldOrder(13)]
		[FieldNullValue(typeof (string), "")]
		public string Priority;
		[FieldOrder(14)]
		[FieldNullValue(typeof (string), "")]
		public string Address;
		[FieldOrder(15)]
		[FieldNullValue(typeof (string), "")]
		public string TellabsAtRisk;
		[FieldOrder(16)]
		[FieldNullValue(typeof (string), "")]
		public string Area;
		[FieldOrder(17)]
		[FieldNullValue(typeof (string), "")]
		public string NSN_Status;
		[FieldOrder(18)]
		[FieldNullValue(typeof (string), "")]
		public string NOC2G;
		[FieldOrder(19)]
		[FieldNullValue(typeof (string), "")]
		public string NOC3G;
		[FieldOrder(20)]
		[FieldNullValue(typeof (string), "")]
		public string NOC4G;
		[FieldOrder(21)]
		[FieldNullValue(typeof (string), "")]
		public string VF_Region;
		[FieldOrder(22)]
		[FieldNullValue(typeof (string), "")]
		public string Special_Id;
		[FieldOrder(23)]
		[FieldNullValue(typeof (string), "")]
		public string Special;
		[FieldOrder(24)]
//		[FieldConverter(ConverterKind.Date, "dd-MMM-yy")]
//		[FieldNullValue(typeof (DateTime), "01-01-1900")]
		public string Special_Start;
		[FieldOrder(25)]
//		[FieldConverter(ConverterKind.Date, "dd-MMM-yy")]
//		[FieldNullValue(typeof (DateTime), "01-01-1900")]
		public string Special_End;
		[FieldOrder(26)]
		[FieldNullValue(typeof (string), "")]
		public string VIP;
		[FieldOrder(27)]
		[FieldNullValue(typeof (string), "")]
		public string Site_Share_Operator;
		[FieldOrder(28)]
		[FieldNullValue(typeof (string), "")]
		public string Site_Share_Site_Id;
		[FieldOrder(29)]
		[FieldNullValue(typeof (string), "")]
		public string Site_Access;
		[FieldOrder(30)]
		[FieldNullValue(typeof (string), "")]
		public string Site_Type;
		[FieldOrder(31)]
		[FieldNullValue(typeof (string), "")]
		public string Site_Subtype;
		[FieldOrder(32)]
		[FieldConverter(ConverterKind.Boolean, "Yes", "No")]
		[FieldNullValue(typeof (bool), "false")]
		public bool Paknet_Fitted;
		[FieldOrder(33)]
		[FieldConverter(ConverterKind.Boolean, "Yes", "No")]
		[FieldNullValue(typeof (bool), "false")]
		public bool Vodapage_Fitted;
		[FieldOrder(34)]
		[FieldNullValue(typeof (string), "")]
		public string DC_STATUS;
		[FieldOrder(35)]
//		[FieldConverter(ConverterKind.Date, "dd-MMM-yyyy")]
//		[FieldNullValue(typeof (DateTime), "01-01-1900")]
		[FieldNullValue(typeof (string), "")]
		public string DC_Timestamp;
		[FieldOrder(36)]
		[FieldNullValue(typeof (string), "")]
		public string Cooling_Status;
		[FieldOrder(37)]
//		[FieldConverter(ConverterKind.Date, "dd-MMM-yyyy")]
//		[FieldNullValue(typeof (DateTime), "01-01-1900")]
		[FieldNullValue(typeof (string), "")]
		public string Cooling_Timestamp;
		[FieldOrder(38)]
		[FieldNullValue(typeof (string), "")]
		public string Key_Information;
		[FieldOrder(39)]
		[FieldNullValue(typeof (string), "")]
		public string EF_HealthAndSafety;
		[FieldOrder(40)]
		[FieldNullValue(typeof (string), "")]
		public string Switch2G;
		[FieldOrder(41)]
		[FieldNullValue(typeof (string), "")]
		public string Switch3G;
		[FieldOrder(42)]
		[FieldNullValue(typeof (string), "")]
		public string Switch4G;
		[FieldOrder(43)]
		[FieldNullValue(typeof (string), "")]
		public string DRSwitch2G;
		[FieldOrder(44)]
		[FieldNullValue(typeof (string), "")]
		public string DRSwitch3G;
		[FieldOrder(45)]
		[FieldNullValue(typeof (string), "")]
		public string DRSwitch4G;
		[FieldOrder(46)]
		[FieldNullValue(typeof (string), "")]
		public string MTX2G;
		[FieldOrder(47)]
		[FieldNullValue(typeof (string), "")]
		public string MTX3G;
		[FieldOrder(48)]
		[FieldNullValue(typeof (string), "")]
		public string MTX4G;
		[FieldOrder(49)]
		[FieldNullValue(typeof (string), "")]
		public string IP_2G_I;
		[FieldOrder(50)]
		[FieldNullValue(typeof (string), "")]
		public string IP_2G_E;
		[FieldOrder(51)]
		[FieldNullValue(typeof (string), "")]
		public string IP_3G_I;
		[FieldOrder(52)]
		[FieldNullValue(typeof (string), "")]
		public string IP_3G_E;
		[FieldOrder(53)]
		[FieldNullValue(typeof (string), "")]
		public string IP_4G_I;
		[FieldOrder(54)]
		[FieldNullValue(typeof (string), "")]
		public string IP_4G_E;
		[FieldOrder(55)]
		[FieldNullValue(typeof (string), "")]
		public string Vendor_2G;
		[FieldOrder(56)]
		[FieldNullValue(typeof (string), "")]
		public string Vendor_3G;
		[FieldOrder(57)]
		[FieldNullValue(typeof (string), "")]
		public string Vendor_4G;
		[FieldOrder(58)]
//		[FieldConverter(ConverterKind.Date, "dd-MM-yyyy")]
//		[FieldNullValue(typeof (DateTime), "01-01-1900")]
		[FieldNullValue(typeof (string), "")]
		public string Date;
		[FieldOrder(59)]
		[FieldNullValue(typeof (string), "")]
		public string MTX_Related;
		
		public Site2()
		{
		}
	}
}
