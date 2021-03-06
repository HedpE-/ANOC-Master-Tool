﻿/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 08/01/2017
 * Time: 02:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using appCore.Settings;

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
		
		public Roles Role {
			get;
			private set;
		}
		
		public SingleShift(string name, string shift, DateTime date) {
			Name = name;
			Shift = shift;
			Date = date;
			if(DB.Databases.shiftsFile.ShiftLeaders.FindIndex(s => string.Equals(s, name, StringComparison.OrdinalIgnoreCase)) > -1)
				Role = Roles.ShiftLeader;
			else {
				if(DB.Databases.shiftsFile.TEF.FindIndex(s => string.Equals(s, name, StringComparison.OrdinalIgnoreCase)) > -1)
					Role = Roles.TEF;
				else {
					if(DB.Databases.shiftsFile.ExternalAlarms.FindIndex(s => string.Equals(s, name, StringComparison.OrdinalIgnoreCase)) > -1)
						Role = Roles.ExternalAlarms;
					else
                    {
                        if (DB.Databases.shiftsFile.Reporting.FindIndex(s => string.Equals(s, name, StringComparison.OrdinalIgnoreCase)) > -1)
                            Role = Roles.Reporting;
                        else
                        {
                            if (DB.Databases.shiftsFile.RAN.FindIndex(s => string.Equals(s, name, StringComparison.OrdinalIgnoreCase)) > -1)
                                Role = Roles.RAN;
                            else
                            {
                                if (DB.Databases.shiftsFile.APT.FindIndex(s => string.Equals(s, name, StringComparison.OrdinalIgnoreCase)) > -1)
                                    Role = Roles.APT;
                            }
                        }
					}
				}
			}
		}

        public override string ToString()
        {
            return Name + " - " + Shift;
        }
    }
}
