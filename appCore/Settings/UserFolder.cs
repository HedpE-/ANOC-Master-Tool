/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 18-08-2016
 * Time: 09:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
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
				UsernameFolder = value == null ? null : new DirectoryInfo(_userFolder.FullName + @"\" + CurrentUser.UserName);
			}
		}
		static DirectoryInfo _usernameFolder;
		static DirectoryInfo UsernameFolder {
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
		
		static System.Timers.Timer LocalDbAutoUpdateTimer = new System.Timers.Timer((60 * 60) * 1 * 1000); // 1 hour in milliseconds
		public static bool ongoingLocalDbFilesUpdate;
		
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
			if(FullName == GlobalProperties.ShareRootDir.FullName || !userFolder.Exists || !UsernameFolder.Exists) {
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
				
				if(!UsernameFolder.Exists)
					UsernameFolder.Create();
				
				if(!LogsFolder.Exists)
					LogsFolder.Create();
			}
			Databases.Initialize();
			
			LocalDbAutoUpdateTimer.Elapsed += delegate {
				UpdateLocalDBFilesCopy();
			};
			
			UpdateLocalDBFilesCopy(true);
			
			LocalDbAutoUpdateTimer.Enabled = true;
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
						DirectoryInfo prevUsernameFolder = new DirectoryInfo(prevFolder.FullName + "\\" + CurrentUser.UserName);
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
								prevUsernameFolder.CopyTo(newFolder.FullName + "\\" + CurrentUser.UserName);
								
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
				if(!UsernameFolder.Exists) {
					UsernameFolder.Create();
					UsernameFolder = new DirectoryInfo(UsernameFolder.FullName);
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
				ongoingLocalDbFilesUpdate = true;
				
				List<Thread> threads = new List<Thread>();
				int finishedThreadsCount = 0;
				Thread thread;
				thread = new Thread(() => {
				                    	// UpdateLocalDBFilesCopy() allcells.csv, allsites.csv & shifts to to UserFolder to minimize share outage impact
				                    	while(Databases.ongoingRemoteDbFilesUpdate) { }
				                    	
				                    	UpdateDBFiles();
				                    	
				                    	finishedThreadsCount++;
				                    });
				thread.Name = "UpdateLocalDBFilesCopy_UpdateDBFiles";
				threads.Add(thread);
				
				thread = new Thread(() => {
				                    	UpdateShiftsFile();
				                    	
				                    	finishedThreadsCount++;
				                    });
				thread.Name = "UpdateLocalDBFilesCopy_UpdateShiftsFile";
				threads.Add(thread);
				
				foreach(Thread th in threads) {
					th.SetApartmentState(ApartmentState.STA);
					th.Start();
				}
				
				while(finishedThreadsCount < threads.Count) { }
				ongoingLocalDbFilesUpdate = false;
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
		
		/// <summary>
		/// Checks and updates the CSV files from the share
		/// </summary>
		static void UpdateDBFiles() {
			if(GlobalProperties.shareAccess) {
				FileInfo source_allsites = new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_sites.csv");
				FileInfo source_allcells = new FileInfo(GlobalProperties.DBFilesDefaultLocation.FullName + @"\all_cells.csv");
				
				List<Thread> threads = new List<Thread>();
				int finishedThreadsCount = 0;
				Thread thread;
				thread = new Thread(() => {
				                    	if(source_allsites.Exists) {
				                    		bool updated = false;
				                    		if(Databases.all_sites != null) {
				                    			if(!Databases.all_sites.Exists || source_allsites.LastWriteTime > Databases.all_sites.LastWriteTime) {
				                    				MainForm.trayIcon.showBalloon("Updating file", "Local all_sites.csv updating...");
				                    				source_allsites.CopyTo(Databases.all_sites.FullName, true);
				                    				updated = true;
				                    			}
				                    		}
				                    		else {
				                    			MainForm.trayIcon.showBalloon("Downloading file", "Downloading all_sites.csv...");
				                    			source_allsites.CopyTo(FullName + @"\all_sites.csv", true);
				                    			updated = true;
				                    		}
				                    		if(updated)
				                    			MainForm.trayIcon.showBalloon("Update complete", "all_sites.csv updated successfully on your UserFolder.");
				                    	}
				                    	
				                    	finishedThreadsCount++;
				                    });
				thread.Name = "UserFolder_UpdateDBFiles_allsites";
				threads.Add(thread);
				
				thread = new Thread(() => {
				                    	if(source_allcells.Exists) {
				                    		bool updated = false;
				                    		if(Databases.all_cells != null) {
				                    			if(!Databases.all_cells.Exists || source_allcells.LastWriteTime > Databases.all_cells.LastWriteTime) {
				                    				MainForm.trayIcon.showBalloon("Updating file", "Local all_cells.csv updating...");
				                    				source_allcells.CopyTo(Databases.all_cells.FullName, true);
				                    				updated = true;
				                    			}
				                    		}
				                    		else {
				                    			MainForm.trayIcon.showBalloon("Downloading file", "Downloading all_cells.csv...");
				                    			source_allcells.CopyTo(FullName + @"\all_cells.csv", true);
				                    			updated = true;
				                    		}
				                    		if(updated)
				                    			MainForm.trayIcon.showBalloon("Update complete", "Local all_cells.csv updated successfully on your UserFolder.");
				                    	}
				                    	
				                    	finishedThreadsCount++;
				                    });
				thread.Name = "UserFolder_UpdateDBFiles_allcells";
				threads.Add(thread);
				
				foreach(Thread th in threads) {
					th.SetApartmentState(ApartmentState.STA);
					th.Start();
				}
				
				while(finishedThreadsCount < threads.Count) { }
			}
			else {
				if(!Databases.all_sites.Exists || !Databases.all_cells.Exists)
					Databases.UpdateSourceDBFiles(true);
			}
		}
		
//		public static void ResetAutoLocalUpdateTimer() {
//			LocalDbAutoUpdateTimer.Stop();
//			LocalDbAutoUpdateTimer.Start();
//		}

		public static FileInfo getDBFile(string pattern) {
			if(!hasDBFile(pattern))
				return null;
			
			FileInfo[] files = userFolder.GetFiles(pattern);
			FileInfo foundFile = null;
			if(files.Length > 1) {
				foreach (FileInfo file in files) {
					if(foundFile == null)
						foundFile = file;
					else {
						if(file.LastWriteTime > foundFile.LastWriteTime)
							foundFile = file;
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
			FileInfo currentShiftsFile = getDBFile("shift*" + DateTime.Now.Year + "*.xlsx");
			FileInfo currentNextYearShiftsFile = getDBFile("shift*" + (DateTime.Now.Year + 1) + "*.xlsx");
			
//			typeof(GlobalProperties).GetField("ShiftsDefaultLocation").SetValue(null, new DirectoryInfo(@"C:\Users\goncarj3\Desktop\Fiddler4Portable"));
			
			if(GlobalProperties.shareAccess) {
				FileInfo[] shiftsFiles = GlobalProperties.ShiftsDefaultLocation.GetFiles("shift*.xlsx");
				if(shiftsFiles.Length > 0) {
					FileInfo newestFile = null;
					FileInfo newestNextYear = null;
					if(shiftsFiles.Length == 1)
						newestFile = shiftsFiles[0];
					else {
						foreach (FileInfo file in shiftsFiles) {
							if(file.Name.Contains(DateTime.Now.Year.ToString())) {
								if(newestFile == null) {
									if(!file.Attributes.ToString().Contains("Hidden") && !file.FullName.StartsWith("~$")) {
//										if(DateTime.Now.Month == 12 && file.Name.Contains((DateTime.Now.Year + 1).ToString())) {
//											if(newestNextYear == null)
//												newestNextYear = file;
//											else {
//												if(file.LastWriteTime > newestNextYear.LastWriteTime)
//													newestNextYear = file;
//											}
//										}
//										else
										newestFile = file;
									}
								}
								else {
									if(file.LastWriteTime > newestFile.LastWriteTime && !file.Attributes.ToString().Contains("Hidden") && !file.FullName.StartsWith("~$")) {
//										if(DateTime.Now.Month == 12 && file.Name.Contains((DateTime.Now.Year + 1).ToString())) {
//											if(newestNextYear == null)
//												newestNextYear = file;
//											else {
//												if(file.LastWriteTime > newestNextYear.LastWriteTime)
//													newestNextYear = file;
//											}
//										}
//										else
										newestFile = file;
									}
								}
							}
							else {
								if(file.Name.Contains((DateTime.Now.Year + 1).ToString())) {
									if(newestNextYear == null)
										newestNextYear = file;
									else {
										if(file.LastWriteTime > newestNextYear.LastWriteTime)
											newestNextYear = file;
									}
								}
							}
						}
					}
					
					if(newestFile != null) {
						if(currentShiftsFile != null) {
							if(newestFile.LastWriteTime > currentShiftsFile.LastWriteTime) {
								if(DateTime.Now.Month != 12 && !newestFile.Name.Contains((DateTime.Now.Year + 1).ToString())) {
								Retry:
									try {
										currentShiftsFile.Delete();
										newestFile.CopyTo(FullName + "\\" + newestFile.Name, true);
										string notificationText = currentShiftsFile.Name == newestFile.Name ?
											"Changes found on the Shifts file, updated on your User Folder" :
											"Next Shifts month available, copied to your User Folder";
										MainForm.trayIcon.showBalloon("Shifts File updated", notificationText);
									}
									catch(Exception e) {
										int errorCode = (int)(e.HResult & 0x0000FFFF);
										DialogResult ans = DialogResult.None;
										if(errorCode == 32)
											ans = ErrorHandling.showShiftsFileInUseDuringFileOperation;
										if(ans == DialogResult.Retry)
											goto Retry;
										
										Databases.shiftsFile = new ShiftsFile(DateTime.Now.Year);
									}
								}
							}
						}
						else {
							newestFile.CopyTo(FullName + "\\" + newestFile.Name);
							MainForm.trayIcon.showBalloon("Shifts File updated", "Shifts file copied to your User Folder");
						}
					}
					if(DateTime.Now.Month == 12) {
						if(newestNextYear != null) {
							if(currentNextYearShiftsFile != null) {
								if(newestNextYear.LastWriteTime > currentNextYearShiftsFile.LastWriteTime) {
									if(DateTime.Now.Month != 12 && !newestNextYear.Name.Contains((DateTime.Now.Year + 1).ToString()))
										currentNextYearShiftsFile.Delete();
									newestNextYear.CopyTo(FullName + "\\" + newestNextYear.Name, true);
									MainForm.trayIcon.showBalloon("Shifts File updated", "Changes found on the " + DateTime.Now.Year + 1 + " Shifts file, updated on your User Folder");
								}
							}
							else {
								newestNextYear.CopyTo(FullName + "\\" + newestNextYear.Name);
								MainForm.trayIcon.showBalloon("Shifts File updated", DateTime.Now.Year + 1 + " Shifts file copied to your User Folder");
							}
						}
					}
				}
			}
			else
				Databases.shiftsFile = new ShiftsFile(DateTime.Now.Year);
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