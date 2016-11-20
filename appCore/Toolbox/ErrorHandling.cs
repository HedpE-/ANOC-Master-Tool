/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 20/11/2016
 * Time: 00:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.UI;
using System;
using System.Windows.Forms;
using System.IO;

namespace appCore
{
	/// <summary>
	/// Description of ErrorHandling.
	/// </summary>
	public static class ErrorHandling
	{
		public static DialogResult showLowSpaceWarningDuringDbFileOperation {
			get {
				return FlexibleMessageBox.Show("Low disk space warning on UserFolder!\n\nAt least 50Mb are needed to complete the operation.\nFree up some space on your UserFolder and retry again.","Low disk space",MessageBoxButtons.RetryCancel,MessageBoxIcon.Error);
			}
		}
		
		public static DialogResult showLowSpaceWarningDuringLogFileOperation {
			get {
				return FlexibleMessageBox.Show("Error writing Log due to low disk space on UserFolder!\n\n\nFree up some space on your UserFolder and retry again.","Low disk space",MessageBoxButtons.RetryCancel,MessageBoxIcon.Error);
			}
		}
		
		public static DialogResult showFileInUseDuringLogFileOperation {
			get {
				return FlexibleMessageBox.Show("Log file is currently in use, please close it and retry.","Error",MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
			}
		}
		
		public static void showIncompatibleDriveWarningOnUserFolderSelection(DriveInfo drive) {
			FlexibleMessageBox.Show("The chosen path for the UserFolder is located on drive " + drive.Name.Substring(0,2) + " which is a " + drive.DriveType + " drive.\n\nPlease choose a path on the hard drive or Removable drive.","Invalid path",MessageBoxButtons.OK,MessageBoxIcon.Error);
		}
		
		public static void showLowSpaceWarningOnUserFolderSelection(DriveInfo drive) {
			FlexibleMessageBox.Show("The chosen path for the UserFolder on drive " + drive.Name.Substring(0,2) + " has only " + (drive.AvailableFreeSpace / Math.Pow(1024, 2)).ToString("n2") + "Mb free.\n\nThis tool needs at least 50Mb free on your UserFolder plus space used by your logs.\n\nPlease make up some free space or choose another folder.","Invalid path",MessageBoxButtons.OK,MessageBoxIcon.Error);
		}
	}
}