/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 08/01/2017
 * Time: 02:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;

namespace appCore.Shifts
{
	/// <summary>
	/// Description of SingleShift.
	/// </summary>
	public class SingleShift {
		public string Name {
			get;
			private set;
		}
		
		public string Shift {
			get;
			private set;
		}
		
		public DateTime Date {
			get;
			private set;
		}
		
		public string Role {
			get;
			private set;
		}
		
		public SingleShift(string name, string shift, DateTime date) {
			Name = name;
			Shift = shift;
			Date = date;
			if(DB.Databases.shiftsFile.ShiftLeaders.FindIndex(s => s.ToUpper() == name.ToUpper()) > -1)
				Role = "Shift Leader";
			else {
				if(DB.Databases.shiftsFile.TEF.FindIndex(s => s.ToUpper() == name.ToUpper()) > -1)
					Role = "TEF";
				else {
					if(DB.Databases.shiftsFile.External.FindIndex(s => s.ToUpper() == name.ToUpper()) > -1)
						Role = "External";
					else {
						if(DB.Databases.shiftsFile.RAN.FindIndex(s => s.ToUpper() == name.ToUpper()) > -1)
							Role = "RAN";
					}
				}
			}
		}
	}
}
