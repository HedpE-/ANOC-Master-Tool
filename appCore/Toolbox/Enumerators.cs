/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 21-07-2016
 * Time: 19:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.ComponentModel;

namespace appCore
{
    public enum Vendors : byte {
			Ericsson,
			ALU,
			Huawei,
			NSN,
			Unknown
		};

    public enum Bearers : byte
    {
        [Description("2G")]
        GSM,
        [Description("3G")]
        UMTS,
        [Description("4G")]
        LTE,
        Unknown
    };

    public enum Operators : byte
    {
        Vodafone,
        Telefonica,
        Unknown
    }

    public enum Roles
    {
        ShiftLeader,
        TEF,
        ExternalAlarms,
        RAN,
        Unknown
    }

    public enum Months : byte
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    };

    public enum UiEnum : byte
    {
        Template,
        Log
    }

    public enum TemplateTypes : byte
    {
        [Description("Troubleshoot Template")]
        Troubleshoot,
        [Description("Failed CRQ")]
        FailedCRQ,
        [Description("Update Template")]
        Update,
        [Description("TX Template")]
        TX,
        [Description("Outage")]
        Outage
    }
}
