﻿/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 30-07-2016
 * Time: 23:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using appCore.Templates.RAN.Types;
using appCore.Settings;

namespace appCore.Templates
{
	/// <summary>
	/// Description of Template.
	/// </summary>
	//Abstract base class for all objects in the Template Logic Layer
	public abstract class Template
	{
		//UniqueId property for every object
		public Guid? UniqueId { get; private set; }
		
		public DateTime GenerationDate { get; set; }
		
		public virtual TemplateTypes LogType { get; set; }
		public virtual string fullLog { get; set; }
		public virtual string Signature { get; protected set; }
		//local member variable which stores the object's UniqueId
		
		//Default constructor
		public Template()
		{
			//create a new unique id for this business object
			UniqueId = Guid.NewGuid();
			GenerationDate = DateTime.Now;
			try {
				Signature = CurrentUser.FullName[1] + " " + CurrentUser.FullName[0] + Environment.NewLine + EnumExtensions.GetDescription(CurrentUser.Department);
                switch(CurrentUser.Department)
                {
                    case Departments.RanTier1:
                        Signature += Environment.NewLine + "ANOC Number: +44 163 569 2067 Op 1";
                        break;
                    case Departments.TxTier1:
                        Signature += Environment.NewLine + "ANOC Number: +44 163 569 2067 Op 2";
                        break;
                    case Departments.CoreTier1:
                        Signature += Environment.NewLine + "ANOC Number: +44 163 569 2067 Op 3";
                        break;
                    case Departments.RanTier2:
                    case Departments.TxTier2:
                        Signature += Environment.NewLine + "ANOC Number: +44 163 569 2069";
                        break;
                    //case Departments.CoreTier2:
                    //    Signature += Environment.NewLine + "ANOC Number: +44 163 569 2069";
                    //    break;
                }
			} catch (Exception) { }
		}
		
		public virtual Troubleshoot ToTroubleshootTemplate() {
			return null;
		}
				
		public virtual FailedCRQ ToFailedCRQTemplate() {
			return null;
		}
		
		public virtual Update ToUpdateTemplate() {
			return null;
		}
		
		public virtual TX ToTxTemplate() {
			return null;
		}
		
		public virtual Outage ToOutageTemplate() {
			return null;
		}
		
		//public enum Filters : byte {
		//	Troubleshoot,
		//	FailedCRQ,
		//	TX,
		//	Update,
		//	Outage,
		//	TicketCount
		//};
	}
}
