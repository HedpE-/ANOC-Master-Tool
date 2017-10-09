/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 18-08-2016
 * Time: 06:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace appCore.Settings
{
    /// <summary>
    /// Description of SettingsFile.
    /// </summary>
    public static class SettingsFile
	{
//		public bool IsMainBackup { get; set; }
//		public int ImageNumber { get; set; }
//		public int IncrementNumber { get; set; }
		static FileInfo settingsFile { get; set; }
		
		public static string FullName {
			get {
				return settingsFile.FullName;
			}
			private set {}
		}
		
		public static bool Exists {
			get {
				return settingsFile.Exists;
			}
			private set {}
		}

        public static DirectoryInfo UserFolderPath
        {
            get
            {
                string temp = SettingsFileHandler("UserFolderPath");
                return string.IsNullOrEmpty(temp) ? null : new DirectoryInfo(temp);
            }
            set
            {
                SettingsFileHandler("UserFolderPath", value.FullName);
            }
        }

        public static string BackgroundImage
        {
			get
            {
				return SettingsFileHandler("BackgroundImage");
			}
			set
            {
				SettingsFileHandler("BackgroundImage", value);
			}
		}
		
		public static string LastRunVersion
        {
			get
            {
				return SettingsFileHandler("LastRunVersion");
			}
			set
            {
				SettingsFileHandler("LastRunVersion", value);
			}
		}
		
		public static string OIUsername
        {
			get
            {
				return SettingsFileHandler("OIUsername");
			}
			set
            {
				SettingsFileHandler("OIUsername", value);
			}
		}
		
		public static string OIPassword
        {
			get
            {
				return SettingsFileHandler("OIPassword");
			}
			set
            {
				SettingsFileHandler("OIPassword", value);
			}
        }

        public static void ResolveSettingsFile()
        {
            DirectoryInfo settingsFolder = ResolveSettingsFolder();

            settingsFile = new FileInfo(settingsFolder.FullName + "\\" + CurrentUser.UserName + ".xml");

            if (settingsFolder.FullName.StartsWith(GlobalProperties.ShareRootDir.FullName))
            {
                if (File.Exists(GlobalProperties.AppDataRootDir.FullName + "\\" + CurrentUser.UserName + ".xml"))
                {
                    if (settingsFile.Exists)
                    {
                        Version appDataLastRunVersion = new Version(SettingsFileHandler("LastRunVersion", new FileInfo(GlobalProperties.AppDataRootDir.FullName + @"\UserSettings\" + CurrentUser.UserName + ".xml")));
                        Version shareLastRunVersion = new Version(LastRunVersion);
                        if (appDataLastRunVersion.CompareTo(shareLastRunVersion) > 0)
                        {
                            File.Copy(GlobalProperties.AppDataRootDir.FullName + "\\" + CurrentUser.UserName + ".xml", settingsFile.FullName, true);
                            File.Delete(GlobalProperties.AppDataRootDir.FullName + "\\" + CurrentUser.UserName + ".xml");
                        }
                    }
                    else
                    {
                        File.Copy(GlobalProperties.AppDataRootDir.FullName + "\\" + CurrentUser.UserName + ".xml", settingsFile.FullName, true);
                        File.Delete(GlobalProperties.AppDataRootDir.FullName + "\\" + CurrentUser.UserName + ".xml");
                    }
                    settingsFile = new FileInfo(settingsFile.FullName);
                }
            }
			//if(GlobalProperties.shareHostAccess.CanRead)
   //         {
			//	settingsFile = new FileInfo(GlobalProperties.ShareRootDir.FullName + @"\UserSettings\" + CurrentUser.UserName + ".xml");
			//	// TODO: CHECK FOR SETTINGS FILE ON DESKTOP AND ASK TO MIGRATE INSTEAD OF CREATING NEW
			//}
			//else
			//	settingsFile = new FileInfo(GlobalProperties.AppDataRootDir.FullName + "\\" + CurrentUser.UserName + ".xml");
			if(!settingsFile.Exists)
				CreateSettingsFile();

			CheckXMLIntegrity();
        }

        public static DirectoryInfo ResolveSettingsFolder()
        {
            DirectoryInfo settingsFolder;
            if (!GlobalProperties.shareHostAccess.CanRead)
            {
                settingsFolder = new DirectoryInfo(GlobalProperties.AppDataRootDir.FullName);

                if (!GlobalProperties.AppDataRootDir.Exists)
                {
                    GlobalProperties.AppDataRootDir.Create();
                    GlobalProperties.AppDataRootDir = new DirectoryInfo(GlobalProperties.AppDataRootDir.FullName);
                }

                settingsFolder = new DirectoryInfo(settingsFolder.FullName);
            }
            else
            {
                if(GlobalProperties.shareHostAccess.CanWrite)
                    settingsFolder = new DirectoryInfo(GlobalProperties.ShareDataDir.FullName + @"\UserSettings");
                else
                {
                    if (!GlobalProperties.AppDataRootDir.Exists)
                    {
                        GlobalProperties.AppDataRootDir.Create();
                        GlobalProperties.AppDataRootDir = new DirectoryInfo(GlobalProperties.AppDataRootDir.FullName);
                    }

                    settingsFolder = new DirectoryInfo(GlobalProperties.AppDataRootDir.FullName);
                }
                //userFolder = new DirectoryInfo(GlobalProperties.ShareRootDir.FullName);
            }

            if (!settingsFolder.Exists)
            {
                settingsFolder.Create();
                settingsFolder = new DirectoryInfo(settingsFolder.FullName);
            }

            return settingsFolder;
        }

        static void CheckXMLIntegrity()
		{
			XmlDocument document = new XmlDocument();			
			document.Load(settingsFile.FullName);

			if (document.GetElementsByTagName("LastRunVersion").Count == 0) {
                XmlElement element = document.CreateElement("LastRunVersion");
				string fileName = Process.GetCurrentProcess().MainModule.FileName;
				element.InnerText = FileVersionInfo.GetVersionInfo(fileName).FileVersion;
                document.DocumentElement.AppendChild(element);
			}
            XmlNodeList elementsByTagName = document.GetElementsByTagName("UserFolderPath");
            if (elementsByTagName.Count > 0)
            {
                if (string.IsNullOrEmpty(elementsByTagName[0].InnerText))
                    elementsByTagName[0].ParentNode.RemoveChild(elementsByTagName[0]);
            }

            elementsByTagName = document.GetElementsByTagName("StartCount");
			if (elementsByTagName.Count > 0)
				elementsByTagName[0].ParentNode.RemoveChild(elementsByTagName[0]);

            elementsByTagName = document.SelectNodes(@"//*[starts-with(name(),'OI')]"); // https://www.w3.org/TR/xpath/#section-Expressions "XML Path Language (XPath)"
            if (elementsByTagName.Count > 0)
            {
                foreach(XmlNode node in elementsByTagName)
                {
                    if (string.IsNullOrEmpty(node.InnerText))
                        node.ParentNode.RemoveChild(node);
                    else
                    {
                        if(node.Name == "OIPassword")
                        {
                            string pass = string.Empty;
                            try
                            {
                                pass = node.InnerText.DecryptText();
                                string encPass = pass.Encrypt();
                                string decPass = encPass.Decrypt();
                                node.InnerText = encPass;
                            }
                            catch { }
                        }
                    }
                }
            }

            elementsByTagName = document.GetElementsByTagName("BackgroundImage");
            if (elementsByTagName.Count > 0)
            {
                if(elementsByTagName[0].InnerText == "Default")
                    elementsByTagName[0].ParentNode.RemoveChild(elementsByTagName[0]);
            }

            document.Save(settingsFile.FullName);
		}

		static void CreateSettingsFile()
		{
			FileVersionInfo fileName = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName);
			new XDocument(
				new object[] {
					new XElement("AMTSettings", new object[] {
                        new XElement("LastRunVersion", fileName.FileVersion)
                    })
				}).Save(settingsFile.FullName);
			settingsFile = new FileInfo(settingsFile.FullName);
		}
		
		public static string SettingsFileHandler(string property, FileInfo settingsFilePath = null)
		{
			XmlDocument document = new XmlDocument();
			document.Load(settingsFilePath == null ? settingsFile.FullName : settingsFilePath.FullName);
			XmlNodeList elementsByTagName = document.GetElementsByTagName(property);
			
			return elementsByTagName.Count > 0 ? elementsByTagName[0].InnerText : string.Empty;
		}
		
		public static void SettingsFileHandler(string property, string newvalue, FileInfo settingsFilePath = null)
		{
            if (string.IsNullOrEmpty(newvalue) && property != "BackgroundImage" && property != "UserFolderPath" && !property.StartsWith("OI"))
                throw new Exception("The XML property doesn't support empty values");

			XmlDocument document = new XmlDocument();
			document.Load(settingsFilePath == null ? settingsFile.FullName : settingsFilePath.FullName);
			XmlNodeList elementsByTagName = document.GetElementsByTagName(property);
            if (elementsByTagName.Count == 0 && !string.IsNullOrEmpty(newvalue))
            {
                XmlNode element = document.CreateNode(XmlNodeType.Element, property, null);
				element.InnerText = newvalue;
				document.DocumentElement.AppendChild(element);
            }
            else
            {
                if(string.IsNullOrEmpty(newvalue))
                    elementsByTagName[0].ParentNode.RemoveChild(elementsByTagName[0]);
                else
                    elementsByTagName[0].InnerText = newvalue;
            }

			document.Save(settingsFilePath == null ? settingsFile.FullName : settingsFilePath.FullName);
		}
	}
}
