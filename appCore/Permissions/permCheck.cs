using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using appCore.Settings;

//Temp
using System.Windows.Forms;


namespace appCore.permChecker
{
	/// <summary>
	/// Class used to set the ANOC Master Tool permissions based on the user department and/or user
	/// Permissions:
	/// 0 - no permissions
	/// 1 - root
	/// 2 - Manager
	/// 3 - Shiftleader
	/// 4 - 1st Line
	/// 5 - 2nd Line
	/// </summary>
	class permCheck
	{
		static string localPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
		// Permissions.xml location
		// HACK: Change location to local file while debugging
		// static string permFile = localPath + "\\Permissions\\permissions.xml";
		static string permFile {
			get {
				return GlobalProperties.shareHostAccess.FullAccess ?
					"\\\\vf-pt\\fs\\ANOC-UK\\ANOC-UK 1st LINE\\1. RAN 1st LINE\\ANOC Master Tool\\data\\Permissions\\permissions.xml" :
					"appCore.Permissions.permissions.xml";
			}
		}
		const int maxUsers = 200;

		public int getUserPerm()
		{
			return retrievePermFromXml(GetUserDetails("Username"));
		}

		public string getPermName(int permissionId)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(permFile);
			foreach (XmlNode node in xmlDoc.ChildNodes[0].ChildNodes[1])
			{
				if (node.Attributes["permission_id"].Value == permissionId.ToString())
					return node.Attributes["name"].Value;
			}
			return "No permission";
		}

		public int currMaxUser()
		{
			return maxUsers;
		}

		public string[,] getUsers()
		{
			string[,] users = new string[maxUsers, 3];
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(permFile);
			int count = 0;
			foreach (XmlNode xmlNode in xmlDoc.ChildNodes[0].ChildNodes[0])
			{
				users[count, 0] = xmlNode.Attributes["name"].Value;
				users[count, 1] = xmlNode.Attributes["username"].Value;
				users[count, 2] = xmlNode.Attributes["permission_id"].Value;
				count++;
			}

			return users;
		}

		public void addUser(string Name, string Username, string permission)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(permFile);
			var newUser = xmlDoc.ChildNodes[0].ChildNodes[0].ChildNodes[0];
			newUser.Attributes["name"].Value = Name;
			newUser.Attributes["username"].Value = Username;
			newUser.Attributes["permission_id"].Value = permission;
			xmlDoc.ChildNodes[0].ChildNodes[0].AppendChild(newUser);
			xmlDoc.Save(permFile);
		}
		
		public void modUser(string Username, int modType, string newVal)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(permFile);
			switch(modType)
			{
					case 0: xmlDoc.SelectSingleNode("/AMTPERM/users/user[@username='" + Username + "']").Attributes["username"].Value = newVal; break; // Mod user name
					case 1: xmlDoc.SelectSingleNode("/AMTPERM/users/user[@username='" + Username + "']").Attributes["name"].Value = newVal; break; // Mod Name
					case 2: xmlDoc.SelectSingleNode("/AMTPERM/users/user[@username='" + Username + "']").Attributes["permission_id"].Value = newVal; break; // Mod Permission
			}
			xmlDoc.Save(permFile);
		}

		private int retrievePermFromXml(string user)
		{
			var xmlDoc = new XmlDocument();
			if(permFile.Contains("appCore")) {
				Assembly assembly = Assembly.GetExecutingAssembly();
				using(Stream stream = assembly.GetManifestResourceStream(permFile))
					xmlDoc.Load(stream);
			}
			else
				xmlDoc.Load(permFile);
			// Search for current user
			foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes[0])
			{
				if (xmlNode.Attributes["username"].Value.Equals(user, StringComparison.InvariantCultureIgnoreCase))
				{
					int perm = 0;
					if (Int32.TryParse(xmlNode.Attributes["permission_id"].Value, out perm))
						return perm;
				}
			}
			// If no user is found in internal resources search in encrypted xml in the share
			// ...

			// If no user is found return no permissions
			return 0;
		}

		public static string GetUserDetails(string detail)
		{
			UserPrincipal current = UserPrincipal.Current;
			if (detail != null)
			{
				switch (detail)
				{
					case "Name":
						if (current.SamAccountName.Contains("Caramelos"))
							return "Gonçalves, Rui";
						if (current.SamAccountName.Contains("Hugo Gonçalves"))
							return "Gonçalves, Hugo";
						return current.DisplayName;
					case "Username":
						return current.SamAccountName;
					case "Department":
						if (current.SamAccountName.Contains("Caramelos"))
							return "1st Line RAN";
						else
						{
							DirectoryEntry underlyingObject = current.GetUnderlyingObject() as DirectoryEntry;
							if (underlyingObject.Properties.Contains("department"))
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
