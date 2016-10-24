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
		
		public static DirectoryInfo UserFolderPath {
			get {
				return new DirectoryInfo(SettingsFileHandler("UserFolderPath"));
			}
			set {
				SettingsFileHandler("UserFolderPath", value.FullName);
			}
		}
		
		public static string BackgroundImage {
			get {
				return SettingsFileHandler("BackgroundImage");
			}
			set {
				SettingsFileHandler("BackgroundImage", value);
			}
		}
		
		public static string LastRunVersion {
			get {
				return SettingsFileHandler("LastRunVersion");
			}
			set {
				SettingsFileHandler("LastRunVersion", value);
			}
		}
		
		public static string OIUsername {
			get {
				return SettingsFileHandler("OIUsername");
			}
			set {
				SettingsFileHandler("OIUsername", value);
			}
		}
		
		public static string OIPassword {
			get {
				return SettingsFileHandler("OIPassword");
			}
			set {
				SettingsFileHandler("OIPassword", value);
			}
		}
		
		public static void ResolveSettingsFile() {
			if(GlobalProperties.shareAccess) {
				settingsFile = new FileInfo(GlobalProperties.ShareRootDir.FullName + @"\UserSettings\" + CurrentUser.userName + ".xml");
				// TODO: CHECK FOR SETTINGS FILE ON DESKTOP AND ASK TO MIGRATE INSTEAD OF CREATING NEW
			}
			else
				settingsFile = new FileInfo(GlobalProperties.FallbackRootDir.FullName + @"\UserSettings\" + CurrentUser.userName + ".xml");
			if(!settingsFile.Exists)
				CreateSettingsFile();
			CheckXMLIntegrity();
		}
		
		static void CheckXMLIntegrity()
		{
			XmlNode documentElement;
			XmlElement element;
			XmlDocument document = new XmlDocument();
			
			document.Load(settingsFile.FullName);
			
			if (document.GetElementsByTagName("UserFolderPath").Count == 0) {
				documentElement = document.DocumentElement;
				element = document.CreateElement("UserFolderPath");
				element.InnerText = UserFolder.FullName;
				documentElement.AppendChild(element);
			}
			if (document.GetElementsByTagName("BackgroundImage").Count == 0) {
				documentElement = document.DocumentElement;
				element = document.CreateElement("BackgroundImage");
				element.InnerText = "Default";
				documentElement.AppendChild(element);
			}
			if (document.GetElementsByTagName("LastRunVersion").Count == 0) {
				documentElement = document.DocumentElement;
				element = document.CreateElement("LastRunVersion");
				string fileName = Process.GetCurrentProcess().MainModule.FileName;
				element.InnerText = FileVersionInfo.GetVersionInfo(fileName).FileVersion;
				documentElement.AppendChild(element);
			}
			if (document.GetElementsByTagName("OIUsername").Count == 0) {
				documentElement = document.DocumentElement;
				element = document.CreateElement("OIUsername");
				documentElement.AppendChild(element);
			}
			if (document.GetElementsByTagName("OIPassword").Count == 0) {
				documentElement = document.DocumentElement;
				element = document.CreateElement("OIPassword");
				documentElement.AppendChild(element);
			}
			XmlNodeList elementsByTagName = document.GetElementsByTagName("StartCount");
			if (elementsByTagName.Count != 0)
				elementsByTagName[0].ParentNode.RemoveChild(elementsByTagName[0]);
			
			document.Save(settingsFile.FullName);
		}

		static void CreateSettingsFile()
		{
			FileVersionInfo fileName = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName);
			new XDocument(
				new object[] {
					new XElement("AMTSettings", new object[] {
					             	new XElement("UserFolderPath", UserFolder.FullName),
					             	new XElement("BackgroundImage", "Default"),
					             	new XElement("LastRunVersion", fileName.FileVersion),
					             	new XElement("OIUsername"),
					             	new XElement("OIPassword")
					             })
				}).Save(settingsFile.FullName);
			settingsFile = new FileInfo(settingsFile.FullName);
		}
		
		public static string SettingsFileHandler(string property)
		{
			XmlDocument document = new XmlDocument();
			document.Load(settingsFile.FullName);
			XmlNodeList elementsByTagName = document.GetElementsByTagName(property);
			
			return elementsByTagName[0].InnerXml;			
		}
		
		public static void SettingsFileHandler(string property, string newvalue)
		{
			XmlDocument document = new XmlDocument();
			document.Load(settingsFile.FullName);
			XmlNodeList elementsByTagName = document.GetElementsByTagName(property);
			
			elementsByTagName[0].InnerXml = newvalue;
			document.Save(settingsFile.FullName);
		}
	}
}
