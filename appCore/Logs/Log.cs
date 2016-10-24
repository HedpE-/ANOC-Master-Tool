/*
 * Created by SharpDevelop.
 * User: Caramelos
 * Date: 23-07-2016
 * Time: 23:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

namespace appCore.Logs
{
	/// <summary>
	/// Description of Log.
	/// </summary>
	//Abstract base class for all business object in the Business Logic Layer
	public abstract class Log
	{
		public virtual string fullLog { get; set; }
		
		//Default constructor
		public Log()
		{
			//create a new unique id for this business object
			UniqueId = Guid.NewGuid();
			GenerationDate = DateTime.Now;
		}

		//UniqueId property for every business object
		public Guid? UniqueId { get; private set; }
		
		public DateTime GenerationDate { get; set; }
		
		public string LogType { get; set; }
		
		public virtual appCore.Templates.Types.TroubleShoot ToTroubleShootTemplate() {
			return null;
		}
		
		public virtual appCore.Templates.Types.FailedCRQ ToFailedCRQTemplate() {
			return null;
		}
		
		public virtual appCore.Templates.Types.Update ToUpdateTemplate() {
			return null;
		}
		
		public virtual appCore.Templates.Types.TX ToTXTemplate() {
			return null;
		}
		
		public virtual appCore.Templates.Types.Outage ToOutageTemplate() {
			return null;
		}
	}
}
