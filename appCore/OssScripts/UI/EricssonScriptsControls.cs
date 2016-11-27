/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 22/11/2016
 * Time: 15:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.UI;
using System;
using System.Windows.Forms;

namespace appCore.OssScripts.UI
{
	/// <summary>
	/// Description of Ericsson.
	/// </summary>
	public class EricssonScriptsControls : Panel
	{
		
		AMTMenuStrip MainMenu = new AMTMenuStrip();
		ToolStripMenuItem generateScriptsToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyLockScriptToolStripMenuItem = new ToolStripMenuItem();
		ToolStripMenuItem copyUnlockScriptToolStripMenuItem = new ToolStripMenuItem();
		
		int paddingLeftRight = 1;
		public int PaddingLeftRight {
			get { return paddingLeftRight; }
			set {
				paddingLeftRight = value;
				DynamicControlsSizesLocations();
			}
		}
		
		int paddingTopBottom = 1;
		public int PaddingTopBottom {
			get { return paddingTopBottom; }
			set {
				paddingTopBottom = value;
				DynamicControlsSizesLocations();
			}
		}
		
		int SelectedTech {
			get;
			set;
		}
		
		public EricssonScriptsControls()
		{
			InitializeComponent();
		}

		void ClearAllControls(object sender, EventArgs e)
		{
//			SiteTextBox.Text = string.Empty;
//			CabinetComboBox.SelectedIndex = 0;
//			GsmRadioButton.Checked = UmtsRadioButton.Checked = LteRadioButton.Checked = false;
//			SiteTextBox.Enabled = false;
			clearToolStripMenuItem.Enabled = false;
		}
		
		void InitializeComponent() {
		}
		
		void DynamicControlsSizesLocations() {
		}
	}
}
