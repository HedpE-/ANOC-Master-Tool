/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 18-08-2016
 * Time: 09:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Windows.Forms;
using appCore.UI;
using appCore.DB;

namespace appCore.Settings
{
	/// <summary>
	/// Description of UserFolder.
	/// </summary>
	public static class UserFolder
	{
		static DirectoryInfo _userFolder;
		static DirectoryInfo userFolder {
			get { return _userFolder; }
			set {
				_userFolder = value;
				usernameFolder = value == null ? null : new DirectoryInfo(_userFolder.FullName + @"\" + CurrentUser.userName);
			}
		}
		static DirectoryInfo _usernameFolder;
		static DirectoryInfo usernameFolder {
			get {
				return _usernameFolder;
			}
			set {
				_usernameFolder = value;
				LogsFolder = value == null ? null : new DirectoryInfo(_usernameFolder.FullName + @"\Logs");
				TempFolder = value == null ? null : new DirectoryInfo(_usernameFolder.FullName + @"\temp");
			}
		}
		
		public static DirectoryInfo LogsFolder { get; private set; }
		
		public static DirectoryInfo TempFolder { get; private set; }
		
		public static string FullName {
			get {
				try {
					return userFolder.FullName;
				}
				catch(Exception) {
					return null;
				}
			}
			private set { }
		}

		public static FileInfo[] GetFiles(string searchPattern)
		{
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			
			return userFolder.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
		}
		
		public static bool ContainsFile(string file) {
			return userFolder.GetFiles(file).Length > 0;
		}
		
		public static void ResolveUserFolder() {
			DirectoryInfo settingsFolder;
			if(!GlobalProperties.shareAccess) {
				if(!GlobalProperties.FallbackRootDir.Exists) {
					GlobalProperties.FallbackRootDir.Create();
					GlobalProperties.FallbackRootDir = new DirectoryInfo(GlobalProperties.FallbackRootDir.FullName);
					GlobalProperties.FallbackRootDir.CreateSubdirectory("UserSettings");
				}
				
				settingsFolder = new DirectoryInfo(GlobalProperties.FallbackRootDir.FullName + @"\UserSettings");
				userFolder = new DirectoryInfo(GlobalProperties.FallbackRootDir.FullName);
			}
			else {
				settingsFolder = new DirectoryInfo(GlobalProperties.ShareRootDir.FullName + @"\UserSettings");
				userFolder = new DirectoryInfo(GlobalProperties.ShareRootDir.FullName);
			}
			
			if(!settingsFolder.Exists) {
				settingsFolder.Create();
				settingsFolder = new DirectoryInfo(settingsFolder.FullName);
			}
		}
		
		public static void Initialize() {
			if(GlobalProperties.shareAccess && SettingsFile.Exists)
				userFolder = SettingsFile.UserFolderPath;
			if(userFolder.FullName == GlobalProperties.ShareRootDir.FullName || !userFolder.Exists || !usernameFolder.Exists) {
				DialogResult result;
				userFolder = null;
				do
				{
					FlexibleMessageBox.Show("Defined user folder not found, please choose default user folder.","ANOC Master Tool",MessageBoxButtons.OK,MessageBoxIcon.Information);
					FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
					result = folderBrowserDialog1.ShowDialog();
					if (result == DialogResult.OK)
					{
						DialogResult ans = FlexibleMessageBox.Show("The following path was chosen:\n\n" + folderBrowserDialog1.SelectedPath + "\n\nContinue with User Folder selection?","Path confirmation",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
						if(ans == DialogResult.Yes) {
							userFolder = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
						}
					}
				} while (userFolder == null);
				
				if(!usernameFolder.Exists) {
					usernameFolder.Create();
				}
				if(!LogsFolder.Exists)
					LogsFolder.Create();
			}
			Databases.Initialize();
			UpdateLocalDBFilesCopy();
		}
		
		public static void Change(DirectoryInfo newFolder) {
			DirectoryInfo prevFolder = userFolder;
			if(!newFolder.Exists) {
				newFolder.Create();
				newFolder = new DirectoryInfo(newFolder.FullName);
			}
			if(prevFolder != null) {
				if(prevFolder.Exists) {
					bool isPrevFallbackFolder = prevFolder.FullName == GlobalProperties.FallbackRootDir.FullName;
					DirectoryInfo prevUsernameFolder = new DirectoryInfo(prevFolder.FullName + "\\" + CurrentUser.userName);
					if(prevUsernameFolder.Exists) {
						DialogResult res;
						res = FlexibleMessageBox.Show("Previous User Folder exists. Do you want to copy all contents to the new Folder?","Copy Contents",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
						if(res == DialogResult.Yes) {
							FlexibleMessageBox.Show("LAST WARNING!" + Environment.NewLine + Environment.NewLine +
							                        "New User Folder is not empty." + Environment.NewLine + Environment.NewLine +
							                        "ANY EXISTING FILES WILL BE OVERWRITTEN IF NOT BACKED UP MANUALLY." + Environment.NewLine + Environment.NewLine +
							                        "Please ensure to backup all data from " + newFolder.FullName +
							                        "\\ before continuing.",
							                        "LAST WARNING!",MessageBoxButtons.OK,MessageBoxIcon.Stop);
							
							prevUsernameFolder.CopyTo(newFolder.FullName + "\\" + CurrentUser.userName);
							
							if(!newFolder.FullName.Contains(prevUsernameFolder.FullName))
								prevUsernameFolder.Delete(true);
							else {
								DirectoryInfo[] subDirs = prevUsernameFolder.GetDirectories();
								foreach (DirectoryInfo dir in subDirs)
									if(!dir.FullName.Contains(prevUsernameFolder.FullName))
										dir.Delete(true);

							}
							
							FileInfo[] newSubFiles = prevFolder.GetFiles();
							foreach (FileInfo file in newSubFiles) {
								if(!file.Attributes.ToString().Contains("Hidden") && !file.FullName.StartsWith("~$"))
									file.CopyTo(newFolder.FullName + "\\" + file.Name, true);
							}
							
							newFolder = new DirectoryInfo(newFolder.FullName);
						}
					}
					
					prevFolder = new DirectoryInfo(prevFolder.FullName);
					if(!isPrevFallbackFolder) {
						DirectoryInfo prevSettingsFolder = new DirectoryInfo(prevFolder.FullName + @"\UserSettings");
						if(prevSettingsFolder.Exists) 
							prevSettingsFolder.Delete(true);
						prevFolder = new DirectoryInfo(prevFolder.FullName);
					}
					else
						if(prevFolder.GetDirectories().Length < 1)
							prevFolder.Delete(true);
				}
			}
			userFolder = newFolder;
			
			if(!LogsFolder.Exists)
				LogsFolder.Create();
			if(!usernameFolder.Exists) {
				usernameFolder.Create();
				usernameFolder = new DirectoryInfo(usernameFolder.FullName);
			}
			SettingsFile.UserFolderPath = userFolder;
			UpdateLocalDBFilesCopy();
		}

		public static void UpdateLocalDBFilesCopy() {
			// UpdateLocalDBFilesCopy() allcells.csv, allsites.csv & shifts to to UserFolder to minimize share outage impact
			UpdateDBFiles();
			UpdateShiftsFile();
		}

		static void UpdateDBFiles() {
			if(GlobalProperties.shareAccess) {
				FileInfo source_allsites = new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv");
				FileInfo source_allcells = new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv");
				
				if(source_allsites.Exists) {
					if(Databases.all_sites != null) {
						if(!Databases.all_sites.Exists || source_allsites.LastWriteTime > Databases.all_sites.LastWriteTime) {
							source_allsites.CopyTo(Databases.all_sites.FullName, true);
//							Databases.RefreshDBFiles("all_sites");
						}
					}
					else {
						source_allsites.CopyTo(userFolder.FullName + @"\all_sites.csv", true);
//						Databases.RefreshDBFiles("all_sites");
					}
				}
				
				if(source_allcells.Exists) {
					if(Databases.all_cells != null) {
						if(!Databases.all_cells.Exists || source_allcells.LastWriteTime > Databases.all_cells.LastWriteTime) {
							source_allcells.CopyTo(Databases.all_cells.FullName, true);
//							Databases.RefreshDBFiles("all_cells");
						}
					}
					else {
						source_allcells.CopyTo(userFolder.FullName + @"\all_cells.csv", true);
//						Databases.RefreshDBFiles("all_cells");
					}
				}
			}
//			else
//				Databases.RefreshDBFiles("all_sites,all_cells");
//			if(!(Databases.all_sites.Exists || Databases.all_cells.Exists) || !GlobalProperties.shareAccess) {
//				if(File.Exists(UserFolder.FullName + @"\all_sites.csv")) {
//					FileInfo a_s = new FileInfo(UserFolder.FullName + @"\all_sites.csv");
//					if(a_s.LastWriteTime > lastSitesDBfile_wrTime) {
//						siteDetailsTable = Tools.GetDataTableFromCsv(a_s.ToString(),true);
//						lastSitesDBfile_wrTime = a_s.LastWriteTime;
//					}
//				}
//				else
//					if(siteDetailsTable == null)
//						siteDetailsTable =  buildSitesTable();
//
//				if(File.Exists(UserFolder.FullName + @"\all_cells.csv")) {
//					FileInfo a_c = new FileInfo(UserFolder.FullName + @"\all_cells.csv");
//					if(a_c.LastWriteTime > lastCellsDBfile_wrTime) {
//						cellDetailsTable = Tools.GetDataTableFromCsv(a_c.ToString(),true);
//						lastCellsDBfile_wrTime = a_c.LastWriteTime;
//					}
//				}
//				else
//					if(cellDetailsTable == null)
//						cellDetailsTable = buildCellsTable();
//			}
		}

		public static FileInfo getDBFile(string file) {
			return hasDBFile(file) ? userFolder.GetFiles(file)[0] : null;
		}

		public static bool hasDBFile(string file) {
			try {
				return userFolder.GetFiles(file).Length > 0;
			}
			catch (Exception) { }
			return false;
		}

		static void UpdateShiftsFile() {
			FileInfo currentShiftsFile = getDBFile("shift*.xlsx");
			
			if(GlobalProperties.shareAccess) {
				FileInfo[] shiftsFiles = GlobalProperties.ShiftsDefaultLocation.GetFiles("shift*.xlsx");
				if(shiftsFiles.Length > 0) {
					FileInfo newestFile = null;
					if(shiftsFiles.Length == 1)
						newestFile = shiftsFiles[0];
					else {
						foreach (FileInfo file in shiftsFiles) {
							if(newestFile == null && !file.Attributes.ToString().Contains("Hidden") && !file.FullName.StartsWith("~$"))
								newestFile = file;
							else {
								if(file.LastWriteTime > newestFile.LastWriteTime && !file.Attributes.ToString().Contains("Hidden") && !file.FullName.StartsWith("~$"))
									newestFile = file;
							}
						}
					}
					
					if(newestFile != null) {
						if(currentShiftsFile != null) {
							if(newestFile.LastWriteTime > currentShiftsFile.LastWriteTime) {
								currentShiftsFile.Delete();
								newestFile.CopyTo(userFolder.FullName + "\\" + newestFile.Name, true);
							}
						}
						else
							newestFile.CopyTo(userFolder.FullName + "\\" + newestFile.Name);
					}
				}
			}
			else
				Databases.shiftsFile = new ShiftsFile();
		}

		public static FileInfo ReadyAMTFailedCRQTempFile() {
			FileInfo path = new FileInfo(UserFolder.FullName + @"\AMTmailTemplate.msg");
			
			if (path.Exists)
				path.Delete();
			File.WriteAllBytes(path.FullName, Resources.AMTmailTemplate);
			return path;
		}

		public static void ReleaseAMTFailedCRQTempFile() {
			FileInfo path = new FileInfo(UserFolder.FullName + @"\AMTmailTemplate.msg");
			if (path.Exists)
				path.Delete();
		}
	}

	public static class DirectoryInfoExtensions
	{
		public static void CopyTo(this DirectoryInfo source, string targetString)
		{
			DirectoryInfo target = new DirectoryInfo(targetString);
			if (!target.Exists)
				target.Create();

			foreach (var file in source.GetFiles())
				file.CopyTo(Path.Combine(target.FullName, file.Name), true);

			foreach (var subdir in source.GetDirectories())
				subdir.CopyTo(target.CreateSubdirectory(subdir.Name).FullName);
		}
	}
}