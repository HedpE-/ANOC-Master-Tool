/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 11-03-2015
 * Time: 03:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace appCore.UI
{
	/// <summary>
	/// Description of NotesForm.
	/// </summary>
	public partial class NotesForm : Form
	{
		public NotesForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			textBox1.Rtf = UI.Resources.Contacts;
			textBox2.Text = UI.Resources.Cells_Prefix;
			textBox3.Rtf = UI.Resources.Useful_Info;
		}
	}
}
