/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 13-11-2014
 * Time: 17:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	sealed class Program
	{
		static string localPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\";
		const string remotePath = "\\\\vf-pt\\fs\\ANOC-UK\\ANOC-UK 1st LINE\\1. RAN 1st LINE\\ANOC Master Tool\\";
		//		const string remotePath = @"D:\ANOC Master Tool\"; // HACK: Override update location
		static NotifyIcon trayIcon = new NotifyIcon();
		static bool networkAccess;

		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// Check if in VF computer
			UserPrincipal current = UserPrincipal.Current;
			if (!current.SamAccountName.Contains("Caramelos") && current.SamAccountName != "Hugo Gonçalves")
			{
				Domain dom;
				try
				{
					dom = Domain.GetComputerDomain();
					if (dom.Name != "internal.vodafone.com")
					{
						MessageBox.Show("ANOC Master Tool not running in VF computer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Environment.Exit(1);
					}
				}
				catch (Exception ex)
				{
					if (ex is ActiveDirectoryObjectNotFoundException)
					{
						MessageBox.Show("ANOC Master Tool not running in VF computer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Environment.Exit(1);
					}
				}
			}

			// Continue
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			trayIcon.Icon = Resource1.MB_0001_vodafone3;
			trayIcon.Text = "ANOC Master Tool";
			trayIcon.ContextMenu = new ContextMenu(new MenuItem[] {
			                                       	new MenuItem("Exit AMT", (s, e) => { Application.Exit(); }),
			                                       });
			trayIcon.Visible = true;

			//string logfile = localPath + "log.txt";

			//if (File.Exists(logfile)) File.Delete(logfile);

			// Check if there is network folder access

			networkAccess = getNetworkAccess(Path.Combine(remotePath, Path.GetRandomFileName()));

			// Check if app is running from network drive
			if (networkAccess && localPath.Contains("\\1. RAN 1st LINE\\ANOC Master Tool"))
			{
				//File.AppendAllText(logfile,"40 - localPath.Contains(\\1. RAN 1st LINE\\ANOC Master Tool) ----- TRUE" + Environment.NewLine + "Exiting...");
				MessageBox.Show("Execution from network share detected!\n\nPlease make a local copy on this computer and try again.", "Terminating", MessageBoxButtons.OK);
				Environment.Exit(1);
			}
			//File.AppendAllText(logfile,"44 - localPath.Contains(\\1. RAN 1st LINE\\ANOC Master Tool) ----- FALSE" + Environment.NewLine + "Checking if appCore.dll is present..." + Environment.NewLine);
			
			bool update = true;
			foreach(string arg in args) {
				if(arg == "-disableUpdates") {
					update = false;
					break;
				}
			}
			
			if(update)
				autoUpdater();

			// TODO: AutoUpdate app while running
			loadUI(args);
		}

		static void loadUI(string[] args)
		{
			Application.Run(new appCore.MainForm(trayIcon, args));
		}

		static void autoUpdater()
		{
			const string Launcherfn = "ANOC Master Tool.exe";
			const string appCorefn = "appCore.dll";

			//			try {
			//				File.Delete(localPath + appCorefn);
			//				MessageBox.Show("Done");
			//			}
			//			catch (Exception e) {
			//				//	File.AppendAllText(logfile,"92 - Update successful ----- FALSE" + Environment.NewLine);
			//				MessageBox.Show("Update failed due to the following exception:\n\n" + e.Message,"Quitting",MessageBoxButtons.OK, MessageBoxIcon.Error);
			//			}

			if (!localPath.Contains("\\bin\\Release\\") && !localPath.Contains("\\bin\\Debug"))
			{// check if tool isn't being ran from IDE, if it is, skip update and dll checks

				// Check if appCore.dll is present

				if (!File.Exists(localPath + appCorefn))
				{
					//File.AppendAllText(logfile,"50 - File.Exists(localPath + appCorefn) ----- FALSE" + Environment.NewLine);
					if (networkAccess)
					{
						if (File.Exists(remotePath + appCorefn))
						{
							//File.AppendAllText(logfile,"52 - File.Exists(remotePath + appCorefn) ----- TRUE" + Environment.NewLine + "Copying..." + Environment.NewLine);
							showBalloon("Downloading resources", "Downloading missing resources");
							File.Delete(localPath + appCorefn);
							File.Copy(remotePath + appCorefn, localPath + appCorefn);
							showBalloon("Download complete", string.Empty);
						}
						else
						{
							//File.AppendAllText(logfile,"57 - File.Exists(remotePath + appCorefn) ----- FALSE" + Environment.NewLine + "Exiting...");
							MessageBox.Show("Critical file appCore.dll is missing on source, please contact the developer (rui.goncalves01@corp.vodafone.pt)", "Terminating", MessageBoxButtons.OK, MessageBoxIcon.Error);
							Environment.Exit(1);
						}
					}
					else
					{
						MessageBox.Show("Network share drive access unnavailable, critical file appCore.dll cannot be downloaded.\nPlease contact the developer (rui.goncalves01@corp.vodafone.pt)", "Terminating", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Environment.Exit(1);
					}
				}
				else
				{
					//File.AppendAllText(logfile,"63 - File.Exists(localPath + appCorefn) ----- TRUE" + Environment.NewLine + "Checking updates..." + Environment.NewLine);

					// Check for updates
					if (networkAccess)
					{
						if (File.Exists(remotePath + Launcherfn))
						{
							//File.AppendAllText(logfile,"69 - File.Exists(remotePath + Launcherfn) ----- TRUE" + Environment.NewLine);
							if (updateChecker(Launcherfn))
							{
								//File.AppendAllText(logfile,"71 - New version found ----- TRUE" + Environment.NewLine + "Exiting...");
								MessageBox.Show("A new version is available, please update your local copy.\n\nYou only need to update the executable file manually, the appCore.dll lib file will be auto updated next time you load the tool.", "Update available", MessageBoxButtons.OK, MessageBoxIcon.Information);
								Process.Start(remotePath);
								Environment.Exit(1);
							}
							//else
							//File.AppendAllText(logfile,"77 - New version found ----- FALSE" + Environment.NewLine);
						}
						//else
						//File.AppendAllText(logfile,"80 - File.Exists(remotePath + Launcherfn) ----- FALSE" + Environment.NewLine);

						if (File.Exists(remotePath + appCorefn))
						{
							//File.AppendAllText(logfile,"84 - File.Exists(remotePath + appCorefn) ----- TRUE" + Environment.NewLine);
							if (updateChecker(appCorefn))
							{
								//File.AppendAllText(logfile,"86 - New version found ----- TRUE" + Environment.NewLine + "Attempting auto update..." + Environment.NewLine);
								showBalloon("Update available", "A new version is available, auto updating...");
								try
								{
									File.Delete(localPath + appCorefn);
									File.Copy(remotePath + appCorefn, localPath + appCorefn, true);
								}
								catch (Exception e)
								{
									//	File.AppendAllText(logfile,"92 - Update successful ----- FALSE" + Environment.NewLine);
									MessageBox.Show("Update failed due to the following exception:\n\n" + e.Message, "Quitting", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
								showBalloon("Update completed", "Update completed successfully!");
								//File.AppendAllText(logfile,"96 - Update successful ----- TRUE" + Environment.NewLine);
							}
							//else
							//File.AppendAllText(logfile,"99 - New version found ----- FALSE" + Environment.NewLine);
						}
						//else
						//File.AppendAllText(logfile,"102 - File.Exists(remotePath + appCorefn) ----- FALSE" + Environment.NewLine + "Starting application...");
					}
				}
			}
		}

		static bool updateChecker(string fn)
		{
			bool NewVersionAvailable = false;

			var localFileVer = FileVersionInfo.GetVersionInfo(localPath + fn);
			var remoteFileVer = FileVersionInfo.GetVersionInfo(remotePath + fn);
			if (remoteFileVer.FileMajorPart > localFileVer.FileMajorPart) NewVersionAvailable = true;
			else
			{
				if ((remoteFileVer.FileMajorPart == localFileVer.FileMajorPart) && (remoteFileVer.FileMinorPart > localFileVer.FileMinorPart)) NewVersionAvailable = true;
				else
				{
					if ((remoteFileVer.FileMajorPart == localFileVer.FileMajorPart) && (remoteFileVer.FileMinorPart == localFileVer.FileMinorPart) && (remoteFileVer.FileBuildPart > localFileVer.FileBuildPart)) NewVersionAvailable = true;
					else
					{
						if ((remoteFileVer.FileMajorPart == localFileVer.FileMajorPart) && (remoteFileVer.FileMinorPart == localFileVer.FileMinorPart) && (remoteFileVer.FileBuildPart == localFileVer.FileBuildPart) && (remoteFileVer.FilePrivatePart > localFileVer.FilePrivatePart)) NewVersionAvailable = true;
					}
				}
			}
			return NewVersionAvailable;
		}

		static void showBalloon(string title, string body)
		{
			if (title != null)
			{
				trayIcon.BalloonTipTitle = title;
			}

			if (body != null)
			{
				trayIcon.BalloonTipText = body;
			}

			trayIcon.ShowBalloonTip(10000);
		}

		static bool getNetworkAccess(string dirPath, bool throwIfFails = false)
		{
			var task = new Task(() =>
			                    {
			                    	try
			                    	{
			                    		File.Create(dirPath, 1, FileOptions.DeleteOnClose);
			                    	}
			                    	catch { }
			                    });
			task.Start();

			networkAccess = task.Wait(10 * 1000);
			task.Dispose();

			//			var networkAccessCheck = new Thread(() => {
			//			                                    	try {
			//			                                    		File.Create(dirPath, 1, FileOptions.DeleteOnClose);
			//			                                    		networkAccess = true;
			//			                                    	} catch {
			//			                                    		if (throwIfFails) {
			//			                                    			throw;
			//			                                    		}
			//			                                    	}
			//			                                    });
			//			networkAccessCheck.IsBackground = true;
			//			networkAccessCheck.Start();
			//			if (!networkAccessCheck.Join(TimeSpan.FromSeconds(10))) {
			//				try {
			//					networkAccessCheck.Abort();
			//				}
			//				catch(ThreadAbortException) { }
			//			}
			return networkAccess;
		}
	}
}
