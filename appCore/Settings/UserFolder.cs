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
using System.Reflection;
using System.Windows.Forms;
using appCore.UI;
using appCore.DB;
//using System.Runtime.InteropServices;

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
		
//		http://stackoverflow.com/questions/14465187/get-available-disk-free-space-for-a-given-path-on-windows
//		[DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
//		[return: MarshalAs(UnmanagedType.Bool)]
//		static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
//		                                      out ulong lpFreeBytesAvailable,
//		                                      out ulong lpTotalNumberOfBytes,
//		                                      out ulong lpTotalNumberOfFreeBytes);

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
			if(SettingsFile.Exists)
				userFolder = SettingsFile.UserFolderPath;
			if(FullName == GlobalProperties.ShareRootDir.FullName || !userFolder.Exists || !usernameFolder.Exists) {
				DialogResult result;
				userFolder = null;
				FlexibleMessageBox.Show("Defined user folder not found, please choose default user folder.","ANOC Master Tool",MessageBoxButtons.OK,MessageBoxIcon.Information);
				do
				{
					FolderBrowserDialog folderBrowserDialog;
					folderBrowserDialog = new FolderBrowserDialog();
					result = folderBrowserDialog.ShowDialog();
					if (result == DialogResult.OK)
					{
						DialogResult ans = FlexibleMessageBox.Show("The following path was chosen:\n\n" + folderBrowserDialog.SelectedPath + "\n\nContinue with User Folder selection?","Path confirmation",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
						if(ans == DialogResult.Yes) {
							DirectoryInfo chosenFolder = new DirectoryInfo(folderBrowserDialog.SelectedPath);
							DriveInfo chosenDrive = new DriveInfo(chosenFolder.Root.FullName);
							if((chosenDrive.DriveType == DriveType.Removable || chosenDrive.DriveType == DriveType.Fixed) && chosenDrive.AvailableFreeSpace > 50 * Math.Pow(1024, 2)) {
								userFolder = chosenFolder;
								SettingsFile.UserFolderPath = userFolder;
							}
							else {
								if(chosenDrive.DriveType != DriveType.Removable && chosenDrive.DriveType != DriveType.Fixed)
									ErrorHandling.showIncompatibleDriveWarningOnUserFolderSelection(chosenDrive);
								else
									ErrorHandling.showLowSpaceWarningOnUserFolderSelection(chosenDrive);
							}
						}
					}
				} while (userFolder == null);
				
				if(!usernameFolder.Exists)
					usernameFolder.Create();
				
				if(!LogsFolder.Exists)
					LogsFolder.Create();
			}
			Databases.Initialize();
			UpdateLocalDBFilesCopy(true);
		}
		
		public static bool Change(DirectoryInfo newFolder) {
			DriveInfo chosenDrive = new DriveInfo(newFolder.Root.FullName);
			
			if((chosenDrive.DriveType == DriveType.Removable || chosenDrive.DriveType == DriveType.Fixed) && chosenDrive.AvailableFreeSpace > 50 * Math.Pow(1024, 2)) {
				DirectoryInfo prevFolder = userFolder;
				
				userFolder = newFolder;
				if(!UpdateLocalDBFilesCopy()) {
					userFolder = prevFolder;
					newFolder.Delete(true);
					return false;
				}
				
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
								res = FlexibleMessageBox.Show("LAST WARNING!" + Environment.NewLine + Environment.NewLine +
								                              "New User Folder is not empty." + Environment.NewLine + Environment.NewLine +
								                              "ANY EXISTING FILES WILL BE OVERWRITTEN IF NOT BACKED UP MANUALLY." + Environment.NewLine + Environment.NewLine +
								                              "Please ensure to backup all data from " + newFolder.FullName +
								                              "\\ before continuing.",
								                              "LAST WARNING!",MessageBoxButtons.OKCancel,MessageBoxIcon.Stop);
								if(res == DialogResult.Cancel) {
									newFolder.Delete(true);
									return false;
								}
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
							else {
								if(!isPrevFallbackFolder) {
									DialogResult ans = FlexibleMessageBox.Show("You chose not to copy the previous UserFolder, do you want to delete it and all it's contents?" +
									                                           Environment.NewLine + Environment.NewLine +
									                                           "WARNING: ANY EXISTING FILES WILL BE DELETED WITHOUT RECOVERY!",
									                                           "Delete previous UserFolder",
									                                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);
									if(ans == DialogResult.Yes)
										prevFolder.Delete(true);
								}
							}
						}
						
						prevFolder = new DirectoryInfo(prevFolder.FullName);
						if(prevFolder.Exists) {
							if(!isPrevFallbackFolder) {
								DirectoryInfo prevSettingsFolder = new DirectoryInfo(prevFolder.FullName + @"\UserSettings");
								if(prevSettingsFolder.Exists) {
									prevSettingsFolder.Delete(true);
									prevFolder = new DirectoryInfo(prevFolder.FullName);
								}
							}
							else
								if(prevFolder.GetDirectories().Length < 1)
									prevFolder.Delete(true);
						}
					}
				}
				
				if(!LogsFolder.Exists)
					LogsFolder.Create();
				if(!usernameFolder.Exists) {
					usernameFolder.Create();
					usernameFolder = new DirectoryInfo(usernameFolder.FullName);
				}
				SettingsFile.UserFolderPath = userFolder;
				
				return true;
			}
			
			if(chosenDrive.DriveType != DriveType.Removable && chosenDrive.DriveType != DriveType.Fixed)
				ErrorHandling.showIncompatibleDriveWarningOnUserFolderSelection(chosenDrive);
			else
				ErrorHandling.showLowSpaceWarningOnUserFolderSelection(chosenDrive);
			
			return false;
		}

		public static bool UpdateLocalDBFilesCopy(bool ForceCloseAppOnError = false) {
		retry:
			try {
				// UpdateLocalDBFilesCopy() allcells.csv, allsites.csv & shifts to to UserFolder to minimize share outage impact
				UpdateDBFiles();
				UpdateShiftsFile();
			}
			catch (Exception e) {
				int errorCode = (int)(e.HResult & 0x0000FFFF);
				
				if(errorCode == 112) { // Low space error code
					DialogResult ans = ErrorHandling.showLowSpaceWarningDuringDbFileOperation;
					if(ans == DialogResult.Retry)
						goto retry;
					
//					if(ForceCloseAppOnError)
//						Application.Exit();
//					else
					return false;
				}
			}
			
			return true;
		}

		static void UpdateDBFiles() {
			if(GlobalProperties.shareAccess) {
				FileInfo source_allsites = new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv");
				FileInfo source_allcells = new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv");
				
				if(source_allsites.Exists) {
					if(Databases.all_sites != null) {
						if(!Databases.all_sites.Exists || source_allsites.LastWriteTime > Databases.all_sites.LastWriteTime)
							source_allsites.CopyTo(Databases.all_sites.FullName, true);
					}
					else
						source_allsites.CopyTo(FullName + @"\all_sites.csv", true);
				}
				
				if(source_allcells.Exists) {
					if(Databases.all_cells != null) {
						if(!Databases.all_cells.Exists || source_allcells.LastWriteTime > Databases.all_cells.LastWriteTime)
							source_allcells.CopyTo(Databases.all_cells.FullName, true);
					}
					else
						source_allcells.CopyTo(FullName + @"\all_cells.csv", true);
				}
			}
			else {
				if(!Databases.all_sites.Exists || !Databases.all_cells.Exists)
					Databases.UpdateSourceDBFiles(true);
			}
		}

		public static FileInfo getDBFile(string pattern) {
			if(!hasDBFile(pattern))
				return null;
			
			FileInfo[] files = userFolder.GetFiles(pattern);
			FileInfo foundFile = null;
			if(files.Length > 1) {
				foreach (FileInfo fi in files) {
					if(DateTime.Now.Month == 12 && fi.Name.Contains(DateTime.Now.Year.ToString())) {
						foundFile = fi;
						break;
					}
				}
			}
			else
				return files[0];
			
			return foundFile;
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
			
//			typeof(GlobalProperties).GetField("ShiftsDefaultLocation").SetValue(null, new DirectoryInfo(@"C:\Users\goncarj3\Desktop\Fiddler4Portable"));
			
			if(GlobalProperties.shareAccess) {
				FileInfo[] shiftsFiles = GlobalProperties.ShiftsDefaultLocation.GetFiles("shift*.xlsx");
				if(shiftsFiles.Length > 0) {
					FileInfo newestFile = null;
					if(shiftsFiles.Length == 1)
						newestFile = shiftsFiles[0];
					else {
						foreach (FileInfo file in shiftsFiles) {
							if(newestFile == null) {
								if(!file.Attributes.ToString().Contains("Hidden") && !file.FullName.StartsWith("~$"))
									newestFile = file;
							}
							else {
								if(file.LastWriteTime > newestFile.LastWriteTime && !file.Attributes.ToString().Contains("Hidden") && !file.FullName.StartsWith("~$"))
									newestFile = file;
							}
						}
					}
					
					if(newestFile != null) {
						if(currentShiftsFile != null) {
							if(newestFile.LastWriteTime > currentShiftsFile.LastWriteTime) {
								if(DateTime.Now.Month != 12 && !newestFile.Name.Contains((DateTime.Now.Year + 1).ToString()))
									currentShiftsFile.Delete();
								newestFile.CopyTo(FullName + "\\" + newestFile.Name, true);
							}
						}
						else
							newestFile.CopyTo(FullName + "\\" + newestFile.Name);
					}
				}
			}
			else
				Databases.shiftsFile = new ShiftsFile();
		}
		
		public static void CreateTempFolder() {
			if(!TempFolder.Exists) {
				TempFolder.Create();
				UserFolder.TempFolder = new DirectoryInfo(TempFolder.FullName);
			}
		}
		
		public static void ClearTempFolder() {
			if(TempFolder.Exists) {
				TempFolder.Delete(true);
				UserFolder.TempFolder = new DirectoryInfo(TempFolder.FullName);
			}
		}

		public static FileInfo ReadyAMTFailedCRQTempFile() {
			CreateTempFolder();
			FileInfo path = new FileInfo(TempFolder.FullName + @"\AMTmailTemplate.msg");
			
			if (path.Exists)
				path.Delete();
			
			File.WriteAllBytes(path.FullName, Resources.AMTmailTemplate);
			return path;
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