/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 05/01/2017
 * Time: 04:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTTextBox.
	/// </summary>
	public class AMTTextBox : TextBox
	{
		protected override void OnKeyDown(KeyEventArgs e) {
			// ReadOnly TextBox Ctrl+A Fix
			if(ReadOnly) {
				if(e.Control && e.KeyCode == Keys.A) {
					// suspend layout to avoid blinking
					SuspendLayout();
					SelectAll();
					Focus();
					ResumeLayout();
				}
			}
		}
	}
}
