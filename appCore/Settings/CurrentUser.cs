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
using System.Linq;

namespace appCore.Settings
{
	/// <summary>
	/// Description of CurrentUser.
	/// </summary>
	public static class CurrentUser
    {        
        public static string UserName
		{
            get
            {
                if (current == null)
                    return UserPrincipal.Current.SamAccountName;

                return current.UserName;
            }
        }

		public static string[] FullName
		{
			get
            {
                return current.FullName;
            }
		}

		public static Departments Department
		{
            get
            {
                return current.Department;
            }
		}

		public static string NetworkDomain
		{
			get
            {
                return current.NetworkDomain;
            }
        }

        public static string Email
        {
            get
            {
                return current.Email;
            }
        }

        public static string VodafoneCountry
        {
            get
            {
                if(string.IsNullOrEmpty(OtherUser))
                {
                    if (UserPrincipal.Current.DisplayName.Contains("Portugal"))
                        return "Vodafone Portugal";

                    return "Vodafone UK";
                }

                if (OtherUser == PT.UserName)
                    return "Vodafone Portugal";

                return "Vodafone UK";
            }
        }

        public static bool HasOICredentials
        {
            get
            {
                return !string.IsNullOrEmpty(SettingsFile.OIUsername);
            }
        }

        public static string ClosureCode
        {
			get
            {
                if (Department == Departments.RanTier1 || Department == Departments.RanTier2)
                    return DB.Databases.shiftsFile.GetClosureCode(FullName[1] + " " + FullName[0]);
                else
                    return string.Empty;
			}
		}

		public static Roles Role
        {
			get
            {
				return DB.Databases.shiftsFile.GetRole(FullName[1] + " " + FullName[0]);
			}
		}
		
		static string OtherUser;

		public static User PT
        {
            get;
            private set;
        }

        public static User UK
        {
            get;
            private set;
        }

        static User current
        {
            get
            {
                if (VodafoneCountry == "Vodafone Portugal")
                    return PT;
                else
                    return UK;
            }
        }

        public static void InitializeUserProperties(string logonAsOtherUser)
		{
            try
            {
                OtherUser = logonAsOtherUser;

                ResolveUserPrincipals();

                SettingsFile.ResolveSettingsFile();

                UserFolder.Initialize();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        static void ResolveUserPrincipals()
        {
            if (string.IsNullOrEmpty(OtherUser))
            {
                if (UserPrincipal.Current.DisplayName.Contains("Portugal"))
                {
                    PT = new User(UserPrincipal.Current);
                    UK = new User(GetUser(UserPrincipal.Current.GivenName, UserPrincipal.Current.Surname, "UK"));
                }
                else
                {
                    UK = new User(UserPrincipal.Current);
                    PT = new User(GetUser(UserPrincipal.Current.GivenName, UserPrincipal.Current.Surname, "PT"));
                }
            }
            else
            {
                using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "internal.vodafone.com"))
                {
                    UserPrincipal temp = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, OtherUser);
                    if (temp.DisplayName.Contains("Portugal"))
                    {
                        PT = new User(temp);
                        UK = new User(GetUser(temp.GivenName, temp.Surname, "UK"));
                    }
                    else
                    {
                        UK = new User(temp);
                        PT = new User(GetUser(temp.GivenName, temp.Surname, "PT"));
                    }
                }
            }
        }

        /// <summary>
        /// .
        /// </summary>
        /// 
        static UserPrincipal GetUser(string givenName, string surname, string vfCountry)
        {
            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "internal.vodafone.com"))
            {
                var queryFilter = new UserPrincipal(ctx) { DisplayName = surname + ", " + givenName + (vfCountry == "UK" ? "*UK*" : "*Portugal*") };
                var searcher = new PrincipalSearcher { QueryFilter = queryFilter };
                var results = searcher.FindAll().ToList();
                if(results.Count > 0)
                {
                    foreach(UserPrincipal foundUser in results)
                    {
                        if (foundUser.Description.Contains("ANOC"))
                            return foundUser;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// .
        /// </summary>
        /// 
        public static User GetUser(string givenName, string surname, string email, string userName, string vfCountry)
        {
            if (string.IsNullOrEmpty(vfCountry))
                throw new Exception("vfCountry cannot be empty");

            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "internal.vodafone.com"))
            {
                if (string.IsNullOrEmpty(surname))
                    surname = "*";

                var queryFilter = new UserPrincipal(ctx);
                queryFilter.DisplayName = surname + ", " + givenName + (vfCountry == "UK" ? "*UK*" : "*Portugal*");
                if(!string.IsNullOrEmpty(email))
                    queryFilter.EmailAddress = email;
                if (!string.IsNullOrEmpty(userName))
                    queryFilter.SamAccountName = userName;

                var searcher = new PrincipalSearcher { QueryFilter = queryFilter };

                var result = searcher.FindOne() as UserPrincipal;

                if(result == null)
                {
                    queryFilter = new UserPrincipal(ctx);
                    queryFilter.DisplayName = surname + ", " + givenName + (vfCountry == "UK" ? "*UK*" : "*Portugal*");
                    if (!string.IsNullOrEmpty(email))
                        queryFilter.EmailAddress = email;
                    if (!string.IsNullOrEmpty(userName))
                        queryFilter.Name = userName;
                    searcher = new PrincipalSearcher { QueryFilter = queryFilter };
                    result = searcher.FindOne() as UserPrincipal;
                }

                return result != null ? new User(searcher.FindOne() as UserPrincipal) : null;
                //var results = searcher.FindAll().ToList();
                //if (results.Count > 0)
                //{
                //    foreach (UserPrincipal foundUser in results)
                //    {
                //        if (foundUser.Description.Contains("ANOC"))
                //            return new User(foundUser);
                //    }
                //}
            }
        }
    }

    public class User
    {
        UserPrincipal activeDirectoryUser;

        public string UserName
        {
            get
            {
                return GetUserDetails("Username");
            }
        }

        public string[] FullName
        {
            get
            {
                string[] fullName = GetUserDetails("Name").Split(' ');
                for (int c = 0; c < fullName.Length; c++)
                    fullName[c] = fullName[c].Replace(",", string.Empty);
                return fullName;
            }
        }

        public Departments Department
        {
            get
            {
                return GetUserDepartment(GetUserDetails("Department"));
            }
        }

        public string NetworkDomain
        {
            get
            {
                return GetUserDetails("NetworkDomain");
            }
        }

        public string Email
        {
            get
            {
                return GetUserDetails("Email");
            }
        }

        public string VodafoneCountry
        {
            get
            {
                if (activeDirectoryUser.DisplayName.Contains("Portugal"))
                    return "Vodafone Portugal";

                return "Vodafone UK";
            }
        }

        public User(UserPrincipal userPrincipal)
        {
            activeDirectoryUser = userPrincipal;
        }

        /// <summary>
        /// Query UserPrincipal's Active Directory details.
        /// Valid queries: "Name", "Username", "Department" or "NetworkDomain"
        /// </summary>
        string GetUserDetails(string detail)
        {
            if (!string.IsNullOrEmpty(detail))
            {
                DirectoryEntry underlyingObject = null;
                switch (detail)
                {
                    case "Name":
                        if (activeDirectoryUser.SamAccountName.Contains("Caramelos"))
                            return "Gonçalves, Rui";
                        if (activeDirectoryUser.SamAccountName.Contains("Hugo Gonçalves"))
                            return "Gonçalves, Hugo";
                        return activeDirectoryUser.Surname + ", " + activeDirectoryUser.GivenName + " " + activeDirectoryUser.MiddleName;
                    case "Username":
                        return activeDirectoryUser.SamAccountName.ToUpper();
                    case "Email":
                        return activeDirectoryUser.EmailAddress;
                    case "VfCountry":
                        return activeDirectoryUser.DisplayName.Contains("Vodafone Portugal") ? "Vodafone Portugal" : "Vodafone UK";
                    case "Department":
                        if (activeDirectoryUser.SamAccountName.Contains("CARAMELOS"))
                            return "1st Line RAN";
                        else
                        {
                            underlyingObject = activeDirectoryUser.GetUnderlyingObject() as DirectoryEntry;
                            string dep = string.Empty;
                            if (underlyingObject.Properties.Contains("department"))
                            {
                                dep = underlyingObject.Properties["department"].Value.ToString();
                                if (string.IsNullOrEmpty(dep) || dep.ToUpper() == "FIRST LINE OPERATIONS UK" || dep.ToUpper() == "SERVICE LEVEL MANAGEMENT" || dep.ToUpper() == "TECHNOLOGY OPERATIONS")
                                {
                                    switch (activeDirectoryUser.EmailAddress.ToLower())
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
                    case "EmployeeNumber":
                        underlyingObject = activeDirectoryUser.GetUnderlyingObject() as DirectoryEntry;
                        if (underlyingObject.Properties.Contains("employeeNumber"))
                            return underlyingObject.Properties["employeeNumber"].Value.ToString();
                        break;
                    case "NetworkDomain":
                        return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                }
            }
            return string.Empty;
        }

        Departments GetUserDepartment(string department = "")
        {
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
                if (department.Contains("CORE"))
                {
                    if (department.Contains("1ST") || department.Contains("FIRST"))
                        return Departments.CoreTier1;
                    else
                        return Departments.CoreTier2;
                }
                else
                {
                    if (department.Contains("TX"))
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
