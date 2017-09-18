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
    public enum Vendors
    {
        [Description("ERICSSON")]
        Ericsson = 1,
        [Description("ALU")]
        ALU = 2,
        [Description("HUAWEI")]
        Huawei = 4,
        [Description("NSN")]
        NSN = 8,
        [Description("Unknown")]
        Unknown = 16
    };

    public enum Bearers
    {
        [Description("2G")]
        GSM = 1,
        [Description("3G")]
        UMTS = 2,
        [Description("4G")]
        LTE = 4,
        [Description("Unknown")]
        Unknown = 8
    };

    public enum Operators
    {
        Vodafone = 1,
        Telefonica = 2,
        Unknown = 4
    }

    public enum Roles
    {
        ShiftLeader = 1,
        TEF = 2,
        ExternalAlarms = 4,
        Reporting = 8,
        RAN = 16,
        APT = 32,
        Unknown = 64
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

    public enum UiEnum
    {
        Template = 1,
        Log = 2
    }

    public enum TemplateTypes
    {
        [Description("Troubleshoot Template")]
        Troubleshoot = 1,
        [Description("Failed CRQ")]
        FailedCRQ = 2,
        [Description("Update Template")]
        Update = 4,
        [Description("TX Template")]
        TX = 8,
        [Description("Outage")]
        Outage = 16
    }

    public enum Departments
    {
        [Description("1st Line RAN Operations")]
        RanTier1 = 1,
        [Description("2nd Line RAN Operations")]
        RanTier2 = 2,
        [Description("1st Line Core&VAS Operations")]
        CoreTier1 = 4,
        [Description("2nd Line Core&VAS Operations")]
        CoreTier2 = 8,
        [Description("1st Line TX Operations")]
        TxTier1 = 16,
        [Description("2nd Line TX Operations")]
        TxTier2 = 32,
        [Description("Unknown Department")]
        Unknown = 64

    }
}
