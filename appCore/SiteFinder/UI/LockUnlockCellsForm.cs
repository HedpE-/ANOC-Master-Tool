/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 03/12/2016
 * Time: 17:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of LockUnlockCellsForm.
	/// </summary>
	public partial class LockUnlockCellsForm : Form
	{
		Site currentSite;
		
		public LockUnlockCellsForm(Site site) {
			currentSite = site;
			InitializeComponent();
			radioButton1.Select();
		}
		
		void RadioButtonsCheckedChanged(object sender, EventArgs e) {
			RadioButton rb = sender as RadioButton;
			listView1.Items.Clear();
			listView1.SuspendLayout();
			
			if(rb.Checked) {
				foreach (Cell cell in currentSite.Cells) {
					ListViewItem lvi = new ListViewItem(
						new[]{ cell.Bearer,
							cell.Name,
							cell.Id,
							cell.BscRnc_Id,
							cell.Vendor.ToString(),
							cell.Noc,
							cell.Locked ? "YES" : "No"
						});
					
					if(rb.Text.StartsWith("Lock")) {
						if(cell.Locked) {
							lvi.ForeColor = SystemColors.GrayText;
							lvi.BackColor = SystemColors.InactiveBorder;
						}
					}
					else {
						if(!cell.Locked) {
							lvi.ForeColor = SystemColors.GrayText;
							lvi.BackColor = SystemColors.InactiveBorder;
						}
					}
					
					listView1.Items.Add(lvi);
				}
				
				foreach (ColumnHeader col in listView1.Columns)
					col.Width = -2;
				
				button1.Text = rb.Text.StartsWith("Lock") ? "Lock\nCells" : "Unlock\nCells";
				checkBox1.Enabled = currentSite.Cells.Filter(Cell.Filters.All_2G).Count > 0;
				checkBox2.Enabled = currentSite.Cells.Filter(Cell.Filters.All_3G).Count > 0;
				checkBox3.Enabled = currentSite.Cells.Filter(Cell.Filters.All_4G).Count > 0;
			}
			
			listView1.ResumeLayout();
		}
		
		void listView1_ItemCheck(object sender, ItemCheckEventArgs e) {
			if(listView1.Items[e.Index].ForeColor == SystemColors.GrayText)
				e.NewValue = e.CurrentValue;
		}
		
		void ListView1ItemChecked(object sender, ItemCheckedEventArgs e) {
			button1.Enabled = listView1.CheckedItems.Count > 0;
		}
	}
}
