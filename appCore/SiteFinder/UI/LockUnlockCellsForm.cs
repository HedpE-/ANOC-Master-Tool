﻿/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 03/12/2016
 * Time: 17:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using appCore.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of LockUnlockCellsForm.
	/// </summary>
	public partial class LockUnlockCellsForm : Form
	{
		Site currentSite;
		int selectable2gCells;
		int selectable3gCells;
		int selectable4gCells;
		int selected2gCells;
		int selected3gCells;
		int selected4gCells;
		
		public LockUnlockCellsForm(Site site) {
			currentSite = site;
			Text = "Site " + currentSite.Id + " Lock/Unlock cells";
			currentSite.UpdateLockedCells();
			currentSite.requestOIData("INCCRQ");
			InitializeComponent();
			radioButton1.Select();
		}
		
		void RadioButtonsCheckedChanged(object sender, EventArgs e) {
			RadioButton rb = sender as RadioButton;
			listView1.Items.Clear();
			listView1.SuspendLayout();
			selectable2gCells = 0;
			selectable3gCells = 0;
			selectable4gCells = 0;
			selected2gCells = 0;
			selected3gCells = 0;
			selected4gCells = 0;
			
			if(rb.Checked) {
				foreach (Cell cell in currentSite.Cells) {
					string ossID;
					if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
						ossID = cell.ENodeB_Id;
					else
						ossID = cell.WBTS_BCF;
					ListViewItem lvi = new ListViewItem(
						new[]{ cell.Bearer,
							cell.Name,
							cell.BscRnc_Id,
							ossID,
							cell.Vendor.ToString(),
							cell.Noc,
							cell.Locked ? "YES" : "No"
						});
					
					if(rb.Text.StartsWith("Lock")) {
						if(cell.Locked || !cell.Noc.Contains("ANOC")) {
							lvi.ForeColor = SystemColors.GrayText;
							lvi.BackColor = SystemColors.InactiveBorder;
						}
						else {
							switch(cell.Bearer) {
								case "2G":
									selectable2gCells++;
									break;
								case "3G":
									selectable3gCells++;
									break;
								case "4G":
									selectable4gCells++;
									break;
							}
						}
					}
					else {
						if(!cell.Locked || !cell.Noc.Contains("ANOC")) {
							lvi.ForeColor = SystemColors.GrayText;
							lvi.BackColor = SystemColors.InactiveBorder;
						}
						else {
							switch(cell.Bearer) {
								case "2G":
									selectable2gCells++;
									break;
								case "3G":
									selectable3gCells++;
									break;
								case "4G":
									selectable4gCells++;
									break;
							}
						}
					}
					
					listView1.Items.Add(lvi);
				}
				
				foreach (ColumnHeader col in listView1.Columns)
					col.Width = -2;
				
				if(rb.Text.StartsWith("Lock")) {
					button1.Text = "Lock\nCells";
					checkBox1.Enabled = currentSite.Cells.Filter(Cell.Filters.All_2G).Where(s => !s.Locked).Count() > 0;
					checkBox2.Enabled = currentSite.Cells.Filter(Cell.Filters.All_3G).Where(s => !s.Locked).Count() > 0;
					checkBox3.Enabled = currentSite.Cells.Filter(Cell.Filters.All_4G).Where(s => !s.Locked).Count() > 0;
					
					foreach(string type in new []{"CRQ","INC"}) {
						DataTable cases;
						cases = type == "INC" ? currentSite.INCs : currentSite.CRQs;
						if(cases.Rows.Count > 0) {
							string query = type == "INC" ? "Status NOT LIKE 'Closed' AND Status NOT LIKE 'Resolved'" :
								"Status = 'Scheduled' OR Status = 'Implementation in Progress'"; // "Status NOT LIKE 'Closed' AND 'Scheduled Start' >= #" + Convert.ToString(DateTime.Now.Date) +"#"; // .ToString("dd-MM-yyyy HH:mm:ss")
							List<DataRow> filteredCases = cases.Select(query).ToList();
							if(type == "CRQ" && filteredCases.Count > 0) {
								for(int c = 0;c < filteredCases.Count;c++) {
									DataRow row = filteredCases[c];
									if(!(row["Scheduled Start"] is DBNull) && !(row["Scheduled End"] is DBNull)) {
										if (!(Convert.ToDateTime(row["Scheduled Start"]) <=  DateTime.Now && Convert.ToDateTime(row["Scheduled End"]) >= DateTime.Now)) {
											filteredCases.RemoveAt(c);
											c--;
										}
									}
								}
							}
							foreach(DataRow row in filteredCases) {
								if(type == "INC")
									comboBox1.Items.Add(row["Incident Ref"]);
								else
									comboBox1.Items.Add(row["Change Ref"]);
							}
						}
					}
				}
				else {
					button1.Text = "Unlock\nCells";
					checkBox1.Enabled = currentSite.Cells.Filter(Cell.Filters.All_2G).Where(s => s.Locked).Count() > 0;
					checkBox2.Enabled = currentSite.Cells.Filter(Cell.Filters.All_3G).Where(s => s.Locked).Count() > 0;
					checkBox3.Enabled = currentSite.Cells.Filter(Cell.Filters.All_4G).Where(s => s.Locked).Count() > 0;
				}
			}
			checkBox1.Checked = checkBox2.Checked = checkBox3.Checked = false;
			comboBox1.Text = amtRichTextBox1.Text = string.Empty;
			listView1.ResumeLayout();
		}
		
		void listView1_ItemCheck(object sender, ItemCheckEventArgs e) {
			if(listView1.Items[e.Index].ForeColor == SystemColors.GrayText)
				e.NewValue = e.CurrentValue;
		}
		
		void ListView1ItemChecked(object sender, ItemCheckedEventArgs e) {
			comboBox1.Enabled = listView1.CheckedItems.Count > 0; // && radioButton1.Checked;
//			switch(e.Item.Text) {
//				case "2G":
//					if(e.Item.Checked)
//						selected2gCells++;
//					else
//						selected2gCells--;
//					break;
//				case "3G":
//					if(e.Item.Checked)
//						selected3gCells++;
//					else
//						selected3gCells--;
//					break;
//				case "4G":
//					if(e.Item.Checked)
//						selected4gCells++;
//					else
//						selected4gCells--;
//					break;
//			}
//				checkBox1.Checked = selected2gCells == selectable2gCells;
//				checkBox2.Checked = selected3gCells == selectable3gCells;
//				checkBox3.Checked = selected4gCells == selectable4gCells;
		}
		
		void CheckBoxesCheckedChanged(object sender, EventArgs e) {
			CheckBox cb = sender as CheckBox;
			var filtered = listView1.Items.Cast<ListViewItem>().Where(s => s.Text == cb.Text);
			foreach(ListViewItem lvi in filtered)
				lvi.Checked = cb.Checked;
		}
		
		void ComboBox1TextUpdate(object sender, EventArgs e) {
			amtRichTextBox1.Enabled = !string.IsNullOrEmpty(comboBox1.Text);
		}
		
		void ComboBox1EnabledChanged(object sender, EventArgs e) {
			amtRichTextBox1.Enabled = !string.IsNullOrEmpty(comboBox1.Text) && comboBox1.Enabled;
		}
		
		void AmtRichTextBox1TextChanged(object sender, EventArgs e) {
			button1.Enabled = !string.IsNullOrEmpty(amtRichTextBox1.Text);
		}
		
		void AmtRichTextBox1EnabledChanged(object sender, EventArgs e) {
			button1.Enabled = !string.IsNullOrEmpty(amtRichTextBox1.Text) && amtRichTextBox1.Enabled;
		}
		
		void Button1Click(object sender, EventArgs e) {
			var filtered = listView1.Items.Cast<ListViewItem>().Where(s => s.Checked);
			List<string> cellsList = new List<string>();
			foreach(ListViewItem lvi in filtered)
				cellsList.Add(lvi.SubItems[1].Text);
			if(button1.Text.StartsWith("Lock"))
				sendLockCellsRequest(cellsList, comboBox1.Text, amtRichTextBox1.Text);
			else
				sendUnlockCellsRequest(cellsList, amtRichTextBox1.Text);
		}
		
		void sendLockCellsRequest(List<string> cellsList, string reference, string comments) {
//			bool manRef = true;
//			foreach(string rf in comboBox1.Items) {
//				if(reference == rf) {
//					manRef = false;
//					break;
//				}
//			}
			bool manRef = !comboBox1.Items.Contains(reference);
			OIConnection.requestPhpOutput("enterlock", currentSite.Id, cellsList, reference, comments, manRef);
			currentSite.UpdateLockedCells();
			RadioButtonsCheckedChanged(radioButton1, null);
		}
		
		void sendUnlockCellsRequest(List<string> cellsList, string comments) {
			OIConnection.requestPhpOutput("cellslocked", currentSite.Id, cellsList, comments);
			currentSite.UpdateLockedCells();
			RadioButtonsCheckedChanged(radioButton2, null);
		}
	}
}