/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 05/01/2017
 * Time: 04:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// Description of AMTTextBox.
	/// </summary>
	public class AMTTextBox : TextBox
	{
        bool errorIssued;
        public bool ErrorIssued
        {
            get
            {
                return errorIssued;
            }
            set
            {
                errorIssued = value;
                Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if(ErrorIssued)
            {
                e.Graphics.DrawRectangle(new Pen(Color.Red), new Rectangle(0, 0, Width - 1, Height - 1));
            }
        }

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
