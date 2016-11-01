/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 03-08-2016
 * Time: 13:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace appCore.Settings
{
    /// <summary>
    /// Description of CurrentUser.
    /// </summary>
    public static class CurrentUser
	{
		public static bool hasOICredentials {
			get;
			private set;
		}
		public static string userName {
			get;
			private set;
		}
		public static string[] fullName {
			get;
			private set;
		}
		public static string department {
			get;
			private set;
		}		
		
		public static void InitializeUserProperties() {
            userName = GetUserDetails("Username");
            fullName = GetUserDetails("Name").Split(' ');
            for (int c = 0; c < fullName.Length; c++)
                fullName[c] = fullName[c].Replace(",", string.Empty);
            department = GetUserDetails("Department").Contains("2nd Line RAN") ? "2nd Line RAN Support" : "1st Line RAN Support";
            UserFolder.ResolveUserFolder();
            SettingsFile.ResolveSettingsFile();
            hasOICredentials = !string.IsNullOrEmpty(SettingsFile.OIUsername);
            UserFolder.Initialize();
		}
		
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
							if(underlyingObject.Properties.Contains("department"))
								return underlyingObject.Properties["department"].Value.ToString();
						}
						break;
					case "NetworkDomain":
						return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
				}
			}
			return string.Empty;
		}
	}
}
