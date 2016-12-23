/*
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
using GlacialComponents.Controls;
using BrightIdeasSoftware.Design;
using appCore.UI;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of LockUnlockCellsForm.
	/// </summary>
	public partial class LockUnlockCellsForm : Form
	{
		Site currentSite;
//		int selectable2gCells;
//		int selectable3gCells;
//		int selectable4gCells;
//		int selected2gCells;
//		int selected3gCells;
//		int selected4gCells;
		GlacialList mylist = new GlacialList();
		
		
		public LockUnlockCellsForm(Site site) {
			currentSite = site;
			Text = "Site " + currentSite.Id + " Lock/Unlock cells";
			currentSite.UpdateLockedCells();
			currentSite.requestOIData("INCCRQ");
			InitializeComponent();
			listView1.Visible = false;
			mylist.Size = listView1.Size;
			mylist.Location = listView1.Location;
			mylist.GridLines = GLGridLines.gridBoth;
			Controls.Add(mylist);
			radioButton1.Select();
		}
		
		void RadioButtonsCheckedChanged(object sender, EventArgs e) {
			RadioButton rb = sender as RadioButton;
			listView1.SuspendLayout();
			listView1.Clear();
			mylist.SuspendLayout();
			mylist.Columns.Clear();
			mylist.Items.Clear();
//			selectable2gCells = 0;
//			selectable3gCells = 0;
//			selectable4gCells = 0;
//			selected2gCells = 0;
//			selected3gCells = 0;
//			selected4gCells = 0;
			
			if(rb.Checked) {
				InitializeGListColumns(rb.Text);
				InitializeListviewColumns(rb.Text);
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
					GLItem item = new GLItem();
					item.SubItems[1].Text = cell.Bearer;
					item.SubItems[2].Text = cell.Name;
					item.SubItems[3].Text = cell.BscRnc_Id;
					item.SubItems[4].Text = ossID;
					item.SubItems[5].Text = cell.Vendor.ToString();
					item.SubItems[6].Text = cell.Noc;
					item.SubItems[7].Text = cell.Locked ? "YES" : "No";
					mylist.Items.Add(item);
					if(rb.Text.StartsWith("Lock")) {
						if(cell.Locked || !cell.Noc.Contains("ANOC")) {
							lvi.ForeColor = SystemColors.GrayText;
							lvi.BackColor = SystemColors.InactiveBorder;
							item.ForeColor = SystemColors.GrayText;
							item.BackColor = SystemColors.InactiveBorder;
						}
//						else {
//							switch(cell.Bearer) {
//								case "2G":
//									selectable2gCells++;
//									break;
//								case "3G":
//									selectable3gCells++;
//									break;
//								case "4G":
//									selectable4gCells++;
//									break;
//							}
//						}
					}
					else {
						if(rb.Text.StartsWith("Unlock") && (!cell.Locked || !cell.Noc.Contains("ANOC"))) {
							lvi.ForeColor = SystemColors.GrayText;
							lvi.BackColor = SystemColors.InactiveBorder;
							item.ForeColor = SystemColors.GrayText;
							item.BackColor = SystemColors.InactiveBorder;
						}
//						else {
//							switch(cell.Bearer) {
//								case "2G":
//									selectable2gCells++;
//									break;
//								case "3G":
//									selectable3gCells++;
//									break;
//								case "4G":
//									selectable4gCells++;
//									break;
//							}
//						}
					}
					
					listView1.Items.Add(lvi);
				}
				
				foreach (ColumnHeader col in listView1.Columns)
					col.Width = -2;
				for(int c = 0;c < mylist.Columns.Count;c++) {
					if(mylist.Columns[c].Text == string.Empty)
						mylist.Columns[c].Width = 19;
					else {
						int textSize = TextRenderer.MeasureText(mylist.Columns[c].Text, mylist.Font).Width;
						foreach (GLItem item in mylist.Items) {
							int tempSize = TextRenderer.MeasureText(item.SubItems[c].Text, mylist.Font).Width;
							if(tempSize > textSize)
								textSize = tempSize;
						}
						mylist.Columns[c].Width = textSize + 7;
					}
				}
				
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
			if(radioButton1.Checked)
				comboBox1.Enabled = listView1.CheckedItems.Count > 0; // && radioButton1.Checked;
			if(radioButton2.Checked)
				amtRichTextBox1.Enabled = listView1.CheckedItems.Count > 0;
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
		
		void InitializeListviewColumns(string radioButtonText) {
			ColumnHeader Tech = new ColumnHeader();
			ColumnHeader CellName = new ColumnHeader();
			ColumnHeader OssId = new ColumnHeader();
			ColumnHeader Switch = new ColumnHeader();
			ColumnHeader Vendor = new ColumnHeader();
			ColumnHeader NOC = new ColumnHeader();
			ColumnHeader Locked = new ColumnHeader();
			
			Tech.Text = "Tech";
			Tech.Width = 38;
			
			CellName.Text = "Cell Name";
			
			Switch.Text = "Switch";
			Switch.Width = 45;
			
			OssId.Text = "OSS ID";
			OssId.Width = 51;
			
			Vendor.Text = "Vendor";
			Vendor.Width = 49;
			
			NOC.Text = "NOC";
			NOC.Width = 39;
			
			Locked.Text = "Locked";
			Locked.TextAlign = HorizontalAlignment.Center;
			Locked.Width = 49;
			
			if(radioButtonText == "Lock Cells")
				listView1.Columns.AddRange(new [] { Tech, CellName, Switch, OssId, Vendor, NOC, Locked });
			else {
				ColumnHeader Reference = new ColumnHeader();
				ColumnHeader CaseStatus = new ColumnHeader();
				ColumnHeader CrqScheduledStart = new ColumnHeader();
				ColumnHeader CrqScheduledEnd = new ColumnHeader();
				ColumnHeader LockedTime = new ColumnHeader();
				ColumnHeader LockedBy = new ColumnHeader();
				ColumnHeader LockComments = new ColumnHeader();
				Reference.Text = "Reference";
				CaseStatus.Text = "Status";
				CrqScheduledStart.Text = "Scheduled Start";
				CrqScheduledEnd.Text = "Scheduled End";
				LockedTime.Text = "Locked Time";
				LockedBy.Text = "Locked By";
				LockComments.Text = "Comments";
				if(radioButtonText == "Unlock Cells")
					listView1.Columns.AddRange(new [] { Tech, CellName, Switch, OssId, Vendor, NOC, Reference, CaseStatus, CrqScheduledStart, CrqScheduledEnd, LockedTime, LockedBy, LockComments, Locked });
				else {
					ColumnHeader UnlockedTime = new ColumnHeader();
					ColumnHeader UnlockedBy = new ColumnHeader();
					ColumnHeader UnlockComments = new ColumnHeader();
					LockComments.Text = "Lock Comments";
					UnlockComments.Text = "Unlock Comments";
					UnlockedTime.Text = "Unlocked Time";
					UnlockedBy.Text = "Unlocked By";
					listView1.Columns.AddRange(new [] { Tech, CellName, Switch, OssId, Vendor, NOC, Reference, CaseStatus, CrqScheduledStart, CrqScheduledEnd, LockedTime, LockedBy, LockComments, UnlockedTime, UnlockedBy, UnlockComments });
				}
			}
		}
		
		void InitializeGListColumns(string radioButtonText) {
			GLColumn select = new GLColumn();
			GLColumn Tech = new GLColumn();
			GLColumn CellName = new GLColumn();
			GLColumn OssId = new GLColumn();
			GLColumn Switch = new GLColumn();
			GLColumn Vendor = new GLColumn();
			GLColumn NOC = new GLColumn();
			GLColumn Locked = new GLColumn();
			
			select.CheckBoxes = true;
			select.Text = string.Empty;
			
			Tech.Text = "Tech";
			Tech.Width = 38;
			
			CellName.Text = "Cell Name";
			
			Switch.Text = "Switch";
			Switch.Width = 45;
			
			OssId.Text = "OSS ID";
			OssId.Width = 51;
			
			Vendor.Text = "Vendor";
			Vendor.Width = 49;
			
			NOC.Text = "NOC";
			NOC.Width = 39;
			
			Locked.Text = "Locked";
			Locked.TextAlignment = ContentAlignment.MiddleCenter;
			Locked.Width = 49;
			
			if(radioButtonText == "Lock Cells")
				mylist.Columns.AddRange(new [] { select, Tech, CellName, Switch, OssId, Vendor, NOC, Locked });
			else {
				GLColumn Reference = new GLColumn();
				GLColumn CaseStatus = new GLColumn();
				GLColumn CrqScheduledStart = new GLColumn();
				GLColumn CrqScheduledEnd = new GLColumn();
				GLColumn LockedTime = new GLColumn();
				GLColumn LockedBy = new GLColumn();
				GLColumn LockComments = new GLColumn();
				Reference.Text = "Reference";
				CaseStatus.Text = "Status";
				CrqScheduledStart.Text = "Scheduled Start";
				CrqScheduledEnd.Text = "Scheduled End";
				LockedTime.Text = "Locked Time";
				LockedBy.Text = "Locked By";
				LockComments.Text = "Comments";
				LockComments.Width = 100;
				if(radioButtonText == "Unlock Cells")
					mylist.Columns.AddRange(new [] { select, Tech, CellName, Switch, OssId, Vendor, NOC, Locked, Reference, CaseStatus, CrqScheduledStart, CrqScheduledEnd, LockedTime, LockedBy, LockComments });
				else {
					GLColumn UnlockedTime = new GLColumn();
					GLColumn UnlockedBy = new GLColumn();
					GLColumn UnlockComments = new GLColumn();
					LockComments.Text = "Lock Comments";
					UnlockComments.Width = 100;
					UnlockComments.Text = "Unlock Comments";
					UnlockedTime.Text = "Unlocked Time";
					UnlockedBy.Text = "Unlocked By";
					mylist.Columns.AddRange(new [] { Tech, CellName, Switch, OssId, Vendor, NOC, Reference, CaseStatus, CrqScheduledStart, CrqScheduledEnd, LockedTime, LockedBy, LockComments, UnlockedTime, UnlockedBy, UnlockComments });
				}
			}
		}
	}
}