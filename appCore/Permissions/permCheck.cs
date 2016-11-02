using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Collections.Generic;
using System.Linq;
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
        public int getUserPerm()
        {
            return retrievePermFromXml(GetUserDetails("Username"));
        }

        private int retrievePermFromXml(string user)
        {
            XmlDocument xmlDoc = new XmlDocument();

            // Load XML from internal resources
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "appCore.Permissions.permissions.xml";
            xmlDoc.Load(assembly.GetManifestResourceStream(resourceName));
            // Search for current user
            foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes[0])
            {
                if (xmlNode.Attributes["username"].Value == user)
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
