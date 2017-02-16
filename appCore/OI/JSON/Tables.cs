/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 14/02/2017
 * Time: 17:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace appCore.OI.JSON
{
	public class RootObject
	{
		public List<object> data { get; set; }
		public string memory { get; set; }
		public string message { get; set; }
	}
	
	public class Availability
	{
		public List<AvailabilityRows> data { get; set; }
		public List<string> title { get; set; }
		public string tablename { get; set; }
		public string memory { get; set; }
		public string message { get; set; }
	}
	
	public class Incident
	{
		[JsonProperty("CI_NAME")]
		public string Site { get; set; }
		[JsonProperty("INCIDENT_NUMBER")]
		public string Incident_Ref { get; set; }
		[JsonProperty("SUMMARY")]
		public string Summary { get; set; }
		[JsonProperty("PRIORITY")]
		public string Priority { get; set; }
		[JsonProperty("STATUS")]
		public string Status { get; set; }
		[JsonProperty("ASSIGNED_GROUP")]
		public string Assigned_Group { get; set; }
		[JsonProperty("SUBMIT_DATE")]
		public string Submit_Date { get; set; }
		[JsonProperty("RESOLUTION_CATEGORY_TIER_2")]
		public string Resolution_Category_2 { get; set; }
		[JsonProperty("RESOLUTION_CATEGORY_TIER_3")]
		public string Resolution_Category_3 { get; set; }
		[JsonProperty("RESOLUTION")]
		public string Resolution { get; set; }
		[JsonProperty("RESOLVED_DATE")]
		public string Resolved_Date { get; set; }
	}
	
	public class Change
	{
		[JsonProperty("CI_NAME")]
		public string Site { get; set; }
		[JsonProperty("CRQ")]
		public string Change_Ref { get; set; }
		[JsonProperty("SUMMARY")]
		public string Summary { get; set; }
		[JsonProperty("STATUS")]
		public string Status { get; set; }
		[JsonProperty("START_DT")]
		public string Scheduled_Start { get; set; }
		[JsonProperty("END_DT")]
		public string Scheduled_End { get; set; }
		[JsonProperty("PROGRAMME")]
		public string Programme { get; set; }
		[JsonProperty("PROJECT")]
		public string Project { get; set; }
		[JsonProperty("RETURN_ON_AIR")]
		public string Return_On_Air { get; set; }
		[JsonProperty("TYPE_OF_WORK")]
		public object Type_Of_Work { get; set; }
	}
	
	public class BookIn
	{
		[JsonProperty("SITECODE")]
		public string Site { get; set; }
		[JsonProperty("VISITKEY")]
		public string Visit { get; set; }
		[JsonProperty("NAME")]
		public string Company { get; set; }
		[JsonProperty("VISTI_NAME")]
		public string Engineer { get; set; }
		[JsonProperty("MSISDN")]
		public string Mobile { get; set; }
		[JsonProperty("REFERENCE")]
		public string Reference { get; set; }
		[JsonProperty("TYPE")]
		public string Visit_Type { get; set; }
		[JsonProperty("ARRIVED")]
		public string Arrived { get; set; }
		[JsonProperty("PLANNED")]
		public string Planned_Finish { get; set; }
		[JsonProperty("TIME_TAKEN")]
		public string Time_Taken { get; set; }
		[JsonProperty("TIME_REMANING")]
		public string Time_Remaining { get; set; }
		[JsonProperty("LEFT")]
		public string Departed_Site { get; set; }
	}
	
	public class Alarm
	{
		[JsonProperty("CI_NAME")]
		public string Site { get; set; }
		[JsonProperty("SEVERITY")]
		public string Severity { get; set; }
		[JsonProperty("GROUP")]
		public string Group { get; set; }
		[JsonProperty("GRADE")]
		public string Grade { get; set; }
		[JsonProperty("OCCURRENCE")]
		public string Date_Time { get; set; }
	}
	
	public class AvailabilityRows
	{
		public string COL1 { get; set; }
		public string COL2 { get; set; }
		public string COL3 { get; set; }
		public string COL4 { get; set; }
		public string COL5 { get; set; }
		public string COL6 { get; set; }
		public string COL7 { get; set; }
		public string COL8 { get; set; }
		public string COL9 { get; set; }
		public string COL10 { get; set; }
		public string COL11 { get; set; }
		public string COL12 { get; set; }
		public string COL13 { get; set; }
		public string COL14 { get; set; }
		public string COL15 { get; set; }
		public string COL16 { get; set; }
		public string COL17 { get; set; }
		public string COL18 { get; set; }
		public string COL19 { get; set; }
		public string COL20 { get; set; }
		public string COL21 { get; set; }
		public string COL22 { get; set; }
		public string COL23 { get; set; }
		public string COL24 { get; set; }
		public string COL25 { get; set; }
		public string COL26 { get; set; }
		public string COL27 { get; set; }
		public string COL28 { get; set; }
		public string COL29 { get; set; }
		public string COL30 { get; set; }
		public string COL31 { get; set; }
		public string COL32 { get; set; }
		public string COL33 { get; set; }
		public string COL34 { get; set; }
		public string COL35 { get; set; }
		public string COL36 { get; set; }
		public string COL37 { get; set; }
		public string COL38 { get; set; }
		public string COL39 { get; set; }
		public string COL40 { get; set; }
		public string COL41 { get; set; }
		public string COL42 { get; set; }
		public string COL43 { get; set; }
		public string COL44 { get; set; }
		public string COL45 { get; set; }
		public string COL46 { get; set; }
		public string COL47 { get; set; }
		public string COL48 { get; set; }
		public string COL49 { get; set; }
		public string COL50 { get; set; }
		public string COL51 { get; set; }
		public string COL52 { get; set; }
		public string COL53 { get; set; }
		public string COL54 { get; set; }
		public string COL55 { get; set; }
		public string COL56 { get; set; }
		public string COL57 { get; set; }
		public string COL58 { get; set; }
		public string COL59 { get; set; }
		public string COL60 { get; set; }
		public string COL61 { get; set; }
		public string COL62 { get; set; }
		public string COL63 { get; set; }
		public string COL64 { get; set; }
		public string COL65 { get; set; }
		public string COL66 { get; set; }
		public string COL67 { get; set; }
		public string COL68 { get; set; }
		public string COL69 { get; set; }
		public string COL70 { get; set; }
		public string COL71 { get; set; }
		public string COL72 { get; set; }
		public string COL73 { get; set; }
		public string COL74 { get; set; }
		public string COL75 { get; set; }
		public string COL76 { get; set; }
		public string COL77 { get; set; }
		public string COL78 { get; set; }
		public string COL79 { get; set; }
		public string COL80 { get; set; }
		public string COL81 { get; set; }
		public string COL82 { get; set; }
		public string COL83 { get; set; }
		public string COL84 { get; set; }
		public string COL85 { get; set; }
		public string COL86 { get; set; }
		public string COL87 { get; set; }
		public string COL88 { get; set; }
		public string COL89 { get; set; }
		public string COL90 { get; set; }
		public string COL91 { get; set; }
		public string COL92 { get; set; }
		public string COL93 { get; set; }
		public string COL94 { get; set; }
		public string COL95 { get; set; }
		public string COL96 { get; set; }
		public string COL97 { get; set; }
		public string COL98 { get; set; }
		public string COL99 { get; set; }
		public string COL100 { get; set; }
		public string COL101 { get; set; }
		public string COL102 { get; set; }
		public string COL103 { get; set; }
		public string COL104 { get; set; }
		public string COL105 { get; set; }
		public string COL106 { get; set; }
		public string COL107 { get; set; }
		public string COL108 { get; set; }
		public string COL109 { get; set; }
		public string COL110 { get; set; }
		public string COL111 { get; set; }
		public string COL112 { get; set; }
		public string COL113 { get; set; }
		public string COL114 { get; set; }
		public string COL115 { get; set; }
		public string COL116 { get; set; }
		public string COL117 { get; set; }
		public string COL118 { get; set; }
		public string COL119 { get; set; }
		public string COL120 { get; set; }
		public string COL121 { get; set; }
		public string COL122 { get; set; }
		public string COL123 { get; set; }
		public string COL124 { get; set; }
		public string COL125 { get; set; }
		public string COL126 { get; set; }
		public string COL127 { get; set; }
		public string COL128 { get; set; }
		public string COL129 { get; set; }
		public string COL130 { get; set; }
		public string COL131 { get; set; }
		public string COL132 { get; set; }
		public string COL133 { get; set; }
		public string COL134 { get; set; }
		public string COL135 { get; set; }
		public string COL136 { get; set; }
		public string COL137 { get; set; }
		public string COL138 { get; set; }
		public string COL139 { get; set; }
		public string COL140 { get; set; }
		public string COL141 { get; set; }
		public string COL142 { get; set; }
		public string COL143 { get; set; }
		public string COL144 { get; set; }
		public string COL145 { get; set; }
		public string COL146 { get; set; }
		public string COL147 { get; set; }
		public string COL148 { get; set; }
		public string COL149 { get; set; }
		public string COL150 { get; set; }
		public string COL151 { get; set; }
		public string COL152 { get; set; }
		public string COL153 { get; set; }
		public string COL154 { get; set; }
		public string COL155 { get; set; }
		public string COL156 { get; set; }
		public string COL157 { get; set; }
		public string COL158 { get; set; }
		public string COL159 { get; set; }
		public string COL160 { get; set; }
		public string COL161 { get; set; }
		public string COL162 { get; set; }
		public string COL163 { get; set; }
		public string COL164 { get; set; }
		public string COL165 { get; set; }
		public string COL166 { get; set; }
		public string COL167 { get; set; }
		public string COL168 { get; set; }
		public string COL169 { get; set; }
		public string COL170 { get; set; }
		public string COL171 { get; set; }
		public string COL172 { get; set; }
		public string COL173 { get; set; }
		public string COL174 { get; set; }
		public string COL175 { get; set; }
		public string COL176 { get; set; }
		public string COL177 { get; set; }
		public string COL178 { get; set; }
		public string COL179 { get; set; }
		public string COL180 { get; set; }
		public string COL181 { get; set; }
		public string COL182 { get; set; }
		public string COL183 { get; set; }
		public string COL184 { get; set; }
		public string COL185 { get; set; }
		public string COL186 { get; set; }
		public string COL187 { get; set; }
		public string COL188 { get; set; }
		public string COL189 { get; set; }
		public string COL190 { get; set; }
		public string COL191 { get; set; }
		public string COL192 { get; set; }
		public string COL193 { get; set; }
		public string CI_NAME { get; set; }
		public string CELL { get; set; }
		public string BEARER { get; set; }
	}
	
	public class OiCell
	{
		public string SITE { get; set; }
		public string BEARER { get; set; }
		public string CELL_NAME { get; set; }
		public string CELL_ID { get; set; }
		public string LAC_TAC { get; set; }
		public string BSC_RNC_ID { get; set; }
		public object ENODEB_ID { get; set; }
		public object WBTS_BCF { get; set; }
		public string VENDOR { get; set; }
		public string NOC { get; set; }
		public string COOS { get; set; }
		public string JVCO_ID { get; set; }
		public int LOCK { get; set; }
		public string LOCKED { get; set; }
	}
	
	public class AccessInformation
	{
		public string CI_NAME { get; set; }
		public string KEY_INFORMATION { get; set; }
		public string EF_HEALTHANDSAFETY { get; set; }
		public string POWER { get; set; }
		public string ADDRESS { get; set; }
	}
}
