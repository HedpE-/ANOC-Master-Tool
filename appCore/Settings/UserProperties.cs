/*
 * Created by SharpDevelop.
 * User: Caramelos
 * Date: 03-08-2016
 * Time: 13:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Windows.Forms;
using appCore.Toolbox;

namespace appCore.Settings
{
	/// <summary>
	/// Description of UserPreferences.
	/// </summary>
	public static class UserProperties
	{
		public static bool userHasOICredentials;
		public static SettingsFile settingsFile;
		public static UserFolder userFolder;
		
		/// <summary>
		/// Valid queries: "Name", "Username", "Department" or "NetworkDomain"
		/// </summary>
		public static string GetUserDetails(string detail)
		{
			UserPrincipal current = UserPrincipal.Current;
			if (detail != null)
			{
				switch(detail) {
					case "Name":
						if(current.SamAccountName.Contains("Caramelos"))
							return "Gonçalves, Rui";
						return current.DisplayName;
					case "Username":
						return current.SamAccountName;
					case "Department":
						if(current.SamAccountName.Contains("Caramelos"))
							return "1st Line RAN";
						else {
							DirectoryEntry underlyingObject = current.GetUnderlyingObject() as DirectoryEntry;
							if(underlyingObject.Properties.Contains("department")) {
								return underlyingObject.Properties["department"].Value.ToString();
							}
						}
						break;
					case "NetworkDomain":
						return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
				}
			}
			return string.Empty;
		}
		
		public static void InitializeUserProperties() {
			userFolder = new UserFolder();
			settingsFile = new SettingsFile();
			userFolder.Initialize();
			userHasOICredentials = string.IsNullOrEmpty(settingsFile.OIUsername);
		}
	}
}
