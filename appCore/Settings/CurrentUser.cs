/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 03-08-2016
 * Time: 13:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace appCore.Settings
{
	/// <summary>
	/// Description of CurrentUser.
	/// </summary>
	public static class CurrentUser
    {
        public static bool HasOICredentials
		{
			get;
			private set;
		}
		public static string UserName
		{
            get;
            private set;
        }
		public static string[] FullName
		{
			get;
			private set;
		}
		public static Departments Department
		{
            get { return GetUserDepartment(); }
		}
		public static string NetworkDomain
		{
			get { return GetUserDetails("NetworkDomain"); }
        }
        public static string Email
        {
            get { return GetUserDetails("Email"); }
        }
        public static string VodafoneCountry
        {
            get
            {
                return GetUserDetails("VfCountry");
            }
        }

        public static string ClosureCode {
			get {
                if (Department == Departments.RanTier1 || Department == Departments.RanTier2)
                    return DB.Databases.shiftsFile.GetClosureCode(FullName[1] + " " + FullName[0]);
                else
                    return string.Empty;
			}
		}
		public static Roles Role {
			get {
				return DB.Databases.shiftsFile.GetRole(FullName[1] + " " + FullName[0]);
			}
		}
		
		static string OtherUser;
		static UserPrincipal ActiveDirectoryUser;

		public static void InitializeUserProperties(string logonAsOtherUser)
		{
			OtherUser = logonAsOtherUser;
			UserName = GetUserDetails("Username");
			FullName = GetUserDetails("Name").Split(' ');
			for (int c = 0; c < FullName.Length; c++)
				FullName[c] = FullName[c].Replace(",", string.Empty);
            //Department = GetUserDepartment();
			//UserFolder.ResolveSettingsFolder();
			SettingsFile.ResolveSettingsFile();
			HasOICredentials = !string.IsNullOrEmpty(SettingsFile.OIUsername);
			UserFolder.Initialize();
        }

        /// <summary>
        /// Query current user's Active Directory details
        /// Valid queries: "Name", "Username", "Department" or "NetworkDomain"
        /// </summary>
        public static string GetUserDetails(string detail)
        {
            if (ActiveDirectoryUser == null)
            {
                if (string.IsNullOrEmpty(OtherUser))
                    ActiveDirectoryUser = UserPrincipal.Current;
                else
                {
                    using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "internal.vodafone.com"))
                    {
                        try
                        {
                            ActiveDirectoryUser = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, OtherUser);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }
                }
            }

            return getUserDetail(ActiveDirectoryUser, detail);
        }

        /// <summary>
        /// Query given user's Active Directory details. Identify user by username and/or email
        /// Valid queries: "Name", "Username", "Department" or "NetworkDomain"
        /// </summary>
        public static string GetUserDetails(string detail, string userName, string email)
        {
            UserPrincipal ADUser = null;
            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "internal.vodafone.com"))
            {
                try
                {
                    if (!string.IsNullOrEmpty(userName))
                        ADUser = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, userName);

                    if (ADUser == null && !string.IsNullOrEmpty(email))
                    {
                        UserPrincipal qbeUser = new UserPrincipal(ctx)
                        {
                            EmailAddress = email
                        };

                        // create your principal searcher passing in the QBE principal    
                        using (PrincipalSearcher srch = new PrincipalSearcher(qbeUser))
                        {
                            ADUser = (UserPrincipal)srch.FindOne();
                            // find all matches
                            //foreach (var found in srch.FindAll())
                            //{
                            //    ADUser = (UserPrincipal)found;
                            //}
                        }
                    }
                }
                catch (Exception e)
                {
                    ADUser = UserPrincipal.FindByIdentity(ctx, IdentityType.UserPrincipalName, userName + "@vodafone.com");
                    //var m = e.Message;
                    //throw new Exception(e.Message);
                }
            }

            return getUserDetail(ADUser, detail);
        }
        
        /// <summary>
        /// Query given UserPrincipal's Active Directory details.
        /// Valid queries: "Name", "Username", "Department" or "NetworkDomain"
        /// </summary>
        static string getUserDetail(UserPrincipal userPrincipal, string detail)
        {
            if (!string.IsNullOrEmpty(detail) && ActiveDirectoryUser != null)
            {
                switch (detail)
                {
                    case "Name":
                        if (ActiveDirectoryUser.SamAccountName.Contains("Caramelos"))
                            return "Gonçalves, Rui";
                        if (ActiveDirectoryUser.SamAccountName.Contains("Hugo Gonçalves"))
                            return "Gonçalves, Hugo";
                        return CurrentUser.ActiveDirectoryUser.Surname + ", " + CurrentUser.ActiveDirectoryUser.GivenName + " " + CurrentUser.ActiveDirectoryUser.MiddleName;
                    case "Username":
                        return ActiveDirectoryUser.SamAccountName.ToUpper();
                    case "Email":
                        return ActiveDirectoryUser.EmailAddress;
                    case "VfCountry":
                        return ActiveDirectoryUser.DisplayName.Contains("Vodafone Portugal") ? "Vodafone Portugal" : "Vodafone UK";
                    case "Department":
                        if (ActiveDirectoryUser.SamAccountName.Contains("CARAMELOS"))
                            return "1st Line RAN";
                        else
                        {
                            DirectoryEntry underlyingObject = ActiveDirectoryUser.GetUnderlyingObject() as DirectoryEntry;
                            string dep = string.Empty;
                            if (underlyingObject.Properties.Contains("department"))
                            {
                                dep = underlyingObject.Properties["department"].Value.ToString();
                                if(string.IsNullOrEmpty(dep) || dep.ToUpper() == "FIRST LINE OPERATIONS UK" || dep.ToUpper() == "SERVICE LEVEL MANAGEMENT" || dep.ToUpper() == "TECHNOLOGY OPERATIONS")
                                {
                                    switch(ActiveDirectoryUser.EmailAddress.ToLower())
                                    {
                                        case "joao.bernardo01@corp.vodafone.pt":
                                        case "luis.costa04@corp.vodafone.pt":
                                            dep = "First Line Operations UK - Core&VAS";
                                            break;
                                        case "david.antunes@vodafone.com":
                                        case "francisco.jesus@corp.vodafone.pt":
                                            dep = "A.NOC UK - 2nd Line - Ericsson/ALU";
                                            break;
                                        case "fernanm1@corp.vodafone.pt":
                                        case "mario.loureiro@corp.vodafone.pt":
                                        case "hugo.fernandes@corp.vodafone.pt":
                                        case "tiago.afonso3@vodafone.com":
                                            dep = "First Line Operations UK - RAN";
                                            break;
                                    }
                                }
                                return dep;
                            }
                        }
                        break;
                    case "NetworkDomain":
                        return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                }
            }
            return string.Empty;
        }

        public static Departments GetUserDepartment(string department = "")
        {
            if(string.IsNullOrEmpty(department))
                department = GetUserDetails("Department"); //.Contains("2nd Line RAN") ? "2nd Line RAN Support" : "1st Line RAN Support";
            department = department.ToUpper();

            if (department.Contains("RAN") || department.Contains("ERICSSON/ALU") || department.Contains("HUAWEI/NOKIA"))
            {
                if (department.Contains("1ST") || department.Contains("FIRST"))
                    return Departments.RanTier1;
                else
                    return Departments.RanTier2;
            }
            else
            {
                if(department.Contains("CORE"))
                {
                    if (department.Contains("1ST") || department.Contains("FIRST"))
                        return Departments.CoreTier1;
                    else
                        return Departments.CoreTier2;
                }
                else
                {
                    if(department.Contains("TX"))
                    {
                        if (department.Contains("1ST") || department.Contains("FIRST"))
                            return Departments.TxTier1;
                        else
                            return Departments.TxTier2;
                    }
                }
            }

            return Departments.Unknown;
        }
	}
}
