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
using appCore.UI;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of LockUnlockCellsForm.
	/// </summary>
	public partial class LockUnlockCellsForm2 : Form
	{
		Site currentSite;
		DataTable Table;
		
		public LockUnlockCellsForm2(Site site) {
			currentSite = site;
			Text = "Site " + currentSite.Id + " Lock/Unlock cells";
			currentSite.UpdateLockedCells(true);
			currentSite.requestOIData("INCCRQ");
			InitializeComponent();
			radioButton1.Select();
		}
		
		void RadioButtonsCheckedChanged(object sender, EventArgs e) {
			RadioButton rb = sender as RadioButton;
//			glacialList1.SuspendLayout();
//			glacialList1.Columns.Clear();
//			glacialList1.Items.Clear();
//			selectable2gCells = 0;
//			selectable3gCells = 0;
//			selectable4gCells = 0;
//			selected2gCells = 0;
//			selected3gCells = 0;
//			selected4gCells = 0;
			
			if(rb.Checked) {
				InitializeDataTable(rb.Text);
				
				
				
//				InitializeGLColumns(rb.Text);
//				if(!rb.Text.StartsWith("History")) {
//					foreach (Cell cell in currentSite.Cells) {
//						string ossID;
//						if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
//							ossID = cell.ENodeB_Id;
//						else
//							ossID = cell.WBTS_BCF;
//
//						GLItem item = new GLItem();
//						item.SubItems[1].Text = cell.Bearer;
//						item.SubItems[2].Text = cell.Name;
//						item.SubItems[3].Text = cell.BscRnc_Id;
//						item.SubItems[4].Text = ossID;
//						item.SubItems[5].Text = cell.Vendor.ToString();
//						item.SubItems[6].Text = cell.Noc;
//						item.SubItems[7].Text = cell.Locked ? "YES" : "No";
//						if(rb.Text.StartsWith("Lock")) {
//							if(cell.Locked || !cell.Noc.Contains("ANOC")) {
//								item.ForeColor = SystemColors.GrayText;
//								item.BackColor = SystemColors.InactiveBorder;
//							}
//						}
//						else {
//							if(currentSite.LockedCellsDetails == null)
//								currentSite.UpdateLockedCells(true);
//							List<DataRow> filtered = new List<DataRow>();
//							foreach(DataRow row in currentSite.LockedCellsDetails.Rows) {
//								if(!string.IsNullOrEmpty(row[6].ToString()) && string.IsNullOrEmpty(row[9].ToString()))
//									filtered.Add(row);
//							}
//							DataRow dr = null;
//							if(filtered.Count > 0) {
//								foreach(DataRow row in filtered) {
//									if(row[0].ToString() == cell.Name) {
//										dr = row;
//										break;
//									}
//								}
//								if(dr != null) {
//									item.SubItems[8].Text = dr[2].ToString();
//									item.SubItems[9].Text = dr[3].ToString();
//									item.SubItems[10].Text = dr[4].ToString();
//									item.SubItems[11].Text = dr[5].ToString();
//									item.SubItems[12].Text = dr[1].ToString();
//									item.SubItems[13].Text = dr[6].ToString();
//									item.SubItems[14].Text = dr[7].ToString();
//								}
//							}
//							if(rb.Text.StartsWith("Unlock") && (!cell.Locked || !cell.Noc.Contains("ANOC"))) {
//								item.ForeColor = SystemColors.GrayText;
//								item.BackColor = SystemColors.InactiveBorder;
//							}
//						}
//
//						glacialList1.Items.Add(item);
//					}
//				}
//				else {
//					if(currentSite.LockedCellsDetails == null)
//						currentSite.UpdateLockedCells(true);
//					foreach (DataRow row in currentSite.LockedCellsDetails.Rows) {
//						GLItem item = new GLItem();
//						Cell cell = currentSite.Cells.Find(s => s.Name == row[0].ToString());
//						string ossID = string.Empty;
//						if(cell != null) {
//							if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
//								ossID = cell.ENodeB_Id;
//							else
//								ossID = cell.WBTS_BCF;
//						}
//
//						item.SubItems[0].Text = cell != null ? cell.Bearer : string.Empty;
//						item.SubItems[1].Text = row[0].ToString();
//						item.SubItems[2].Text = cell != null ? cell.BscRnc_Id : string.Empty;
//						item.SubItems[3].Text = ossID;
//						item.SubItems[4].Text = cell != null ? cell.Vendor.ToString() : string.Empty;
//						item.SubItems[5].Text = cell != null ? cell.Noc : string.Empty;
				////						item.SubItems[6].Text = cell != null ? (cell.Locked ? "YES" : "No") : string.Empty;
//						item.SubItems[6].Text = row[2].ToString();
//						item.SubItems[7].Text = row[3].ToString();
//						item.SubItems[8].Text = row[4].ToString();
//						item.SubItems[9].Text = row[5].ToString();
//						item.SubItems[10].Text = row[1].ToString();
//						item.SubItems[11].Text = row[6].ToString();
//						item.SubItems[12].Text = row[7].ToString();
//						item.SubItems[13].Text = row[8].ToString();
//						item.SubItems[14].Text = row[9].ToString();
//						item.SubItems[15].Text = row[10].ToString();
//
//						glacialList1.Items.Add(item);
//					}
//				}
//				for(int c = 0;c < glacialList1.Columns.Count;c++) {
//					if(glacialList1.Columns[c].Text == string.Empty)
//						glacialList1.Columns[c].Width = 19;
//					else {
//						if(glacialList1.Columns[c].Text.Contains("Comments"))
//							glacialList1.Columns[c].Width = 250;
//						else {
//							int textSize = TextRenderer.MeasureText(glacialList1.Columns[c].Text, glacialList1.Font).Width;
//							foreach (GLItem item in glacialList1.Items) {
//								int tempSize = TextRenderer.MeasureText(item.SubItems[c].Text, glacialList1.Font).Width;
//								if(tempSize > textSize)
//									textSize = tempSize;
//							}
//							glacialList1.Columns[c].Width = textSize + 7;
//						}
//					}
//				}
//				for(int c = 0;c < glacialList1.Items.Count;c++) {
////					glacialList1
//				}
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
					if(rb.Text.StartsWith("Unlock")) {
						checkBox1.Enabled = currentSite.Cells.Filter(Cell.Filters.All_2G).Where(s => s.Locked).Count() > 0;
						checkBox2.Enabled = currentSite.Cells.Filter(Cell.Filters.All_3G).Where(s => s.Locked).Count() > 0;
						checkBox3.Enabled = currentSite.Cells.Filter(Cell.Filters.All_4G).Where(s => s.Locked).Count() > 0;
					}
					else
						button1.Enabled = checkBox1.Enabled = checkBox2.Enabled = checkBox3.Enabled = false;
				}
			}
			checkBox1.Checked = checkBox2.Checked = checkBox3.Checked = false;
			comboBox1.Text = amtRichTextBox1.Text = string.Empty;
//			glacialList1.ResumeLayout();
		}
		
		void CheckBoxesCheckedChanged(object sender, EventArgs e) {
			CheckBox cb = sender as CheckBox;
			var filtered = dataGridView1.Rows.Cast<DataGridViewRow>().Where(s => s.Cells["Tech"].Value.ToString() == cb.Text);
			
//			if(filtered.Any())
//				glacialList1.ItemChangedEvent -= GlacialList1ItemChangedEvent;
			foreach(DataGridViewRow dgvr in filtered) {
				DataGridViewCheckBoxCell cell = dgvr.Cells[0] as DataGridViewCheckBoxCell;
				cell.Value = cb.Checked ? cell.TrueValue : cell.FalseValue;
//				if(filtered.Last() == dgvr) {
//					glacialList1.ItemChangedEvent += GlacialList1ItemChangedEvent;
//					gli.SubItems[0].Checked = cb.Checked;
//				}
//				else
//					gli.SubItems[0].Checked = cb.Checked;
			}
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
			var filtered = glacialList1.Items.Cast<GLItem>().Where(s => s.SubItems[0].Checked);
			List<string> cellsList = new List<string>();
			foreach(GLItem gli in filtered)
				cellsList.Add(gli.SubItems[1].Text);
			if(button1.Text.StartsWith("Lock"))
				sendLockCellsRequest(cellsList, comboBox1.Text, amtRichTextBox1.Text);
			else
				sendUnlockCellsRequest(cellsList, amtRichTextBox1.Text);
		}
		
		void sendLockCellsRequest(List<string> cellsList, string reference, string comments) {
			bool manRef = !comboBox1.Items.Contains(reference);
			OIConnection.requestPhpOutput("enterlock", currentSite.Id, cellsList, reference, comments, manRef);
			currentSite.UpdateLockedCells(true);
			RadioButtonsCheckedChanged(radioButton1, null);
		}
		
		void sendUnlockCellsRequest(List<string> cellsList, string comments) {
			OIConnection.requestPhpOutput("cellslocked", currentSite.Id, cellsList, comments);
			currentSite.UpdateLockedCells(true);
			RadioButtonsCheckedChanged(radioButton2, null);
		}
		
		void InitializeDataTable(string radioButtonText) {
			DataColumn Tech = new DataColumn("Tech");
			DataColumn CellName = new DataColumn("Cell Name");
			DataColumn Switch = new DataColumn("Switch");
			DataColumn OssId = new DataColumn("OSS ID");
			DataColumn Vendor = new DataColumn("Vendor");
			DataColumn NOC = new DataColumn("NOC");
			DataColumn Locked = new DataColumn("Locked");
			Table.Columns.AddRange(new [] { Tech, CellName, Switch, OssId, Vendor, NOC });
			if(radioButtonText != "History")
				Table.Columns.Add(Locked);
			
			if(radioButtonText == "Lock Cells") {
				foreach (Cell cell in currentSite.Cells) {
					string ossID;
					if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
						ossID = cell.ENodeB_Id;
					else
						ossID = cell.WBTS_BCF;
					
					DataRow row = Table.NewRow();
					row["Tech"] = cell.Bearer;
					row["Cell Name"] = cell.Name;
					row["Switch"] = cell.BscRnc_Id;
					row["OSS ID"] = ossID;
					row["Vendor"] = cell.Vendor.ToString();
					row["NOC"] = cell.Noc;
					row["Locked"] = cell.Locked ? "YES" : "No";
					
					Table.Rows.Add(row);
				}
			}
			else {
				DataColumn Reference = new DataColumn("Reference");
				DataColumn CaseStatus = new DataColumn("Status");
				DataColumn CrqScheduledStart = new DataColumn("Scheduled Start");
				DataColumn CrqScheduledEnd = new DataColumn("Scheduled End");
				DataColumn LockedTime = new DataColumn("Locked Time");
				DataColumn LockedBy = new DataColumn("Locked By");
				DataColumn LockComments = new DataColumn("Lock Comments");
				Table.Columns.AddRange(new [] { Reference, CaseStatus, CrqScheduledStart, CrqScheduledEnd, LockedTime, LockedBy, LockComments });
				if(radioButtonText == "Unlock Cells") {
					foreach (Cell cell in currentSite.Cells) {
						string ossID;
						if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
							ossID = cell.ENodeB_Id;
						else
							ossID = cell.WBTS_BCF;
						
						DataRow row = Table.NewRow();
						row["Tech"] = cell.Bearer;
						row["Cell Name"] = cell.Name;
						row["Switch"] = cell.BscRnc_Id;
						row["OSS ID"] = ossID;
						row["Vendor"] = cell.Vendor.ToString();
						row["NOC"] = cell.Noc;
						row["Locked"] = cell.Locked ? "YES" : "No";
						
						if(currentSite.LockedCellsDetails == null)
							currentSite.UpdateLockedCells(true);
						
						List<DataRow> filtered = new List<DataRow>();
						foreach(DataRow dr in currentSite.LockedCellsDetails.Rows) {
							if(!string.IsNullOrEmpty(dr[6].ToString()) && string.IsNullOrEmpty(dr[9].ToString()))
								filtered.Add(dr);
						}
						
						DataRow tempRow = null;
						if(filtered.Count > 0) {
							foreach(DataRow dr in filtered) {
								if(dr[0].ToString() == cell.Name) {
									tempRow = dr;
									break;
								}
							}
							if(tempRow != null) {
								row["Reference"] = tempRow[2].ToString();
								row["Status"] = tempRow[3].ToString();
								row["Scheduled Start"] = tempRow[4].ToString();
								row["Scheduled End"] = tempRow[5].ToString();
								row["Locked Time"] = tempRow[1].ToString();
								row["Locked By"] = tempRow[6].ToString();
								row["Lock Comments"] = tempRow[7].ToString();
							}
						}
						
						Table.Rows.Add(row);
					}
				}
				else {
					DataColumn UnlockedTime = new DataColumn("Unlocked Time");
					DataColumn UnlockedBy = new DataColumn("Unlocked By");
					DataColumn UnlockComments = new DataColumn("Unlock Comments");
					Table.Columns.AddRange(new [] { UnlockedTime, UnlockedBy, UnlockComments });
					
					if(currentSite.LockedCellsDetails == null)
						currentSite.UpdateLockedCells(true);
					foreach (DataRow dr in currentSite.LockedCellsDetails.Rows) {
						DataRow row = Table.NewRow();
						Cell cell = currentSite.Cells.Find(s => s.Name == dr[0].ToString());
						string ossID = string.Empty;
						if(cell != null) {
							if(cell.Vendor == SiteFinder.Site.Vendors.NSN && cell.Bearer == "4G")
								ossID = cell.ENodeB_Id;
							else
								ossID = cell.WBTS_BCF;
						}
						
						row["Tech"] = cell != null ? cell.Bearer : string.Empty;
						row["Cell Name"] = dr[0].ToString();
						row["Switch"] = cell != null ? cell.BscRnc_Id : string.Empty;
						row["OSS ID"] = ossID;
						row["Vendor"] = cell != null ? cell.Vendor.ToString() : string.Empty;
						row["NOC"] = cell != null ? cell.Noc : string.Empty;
						
						row["Reference"] = dr[2].ToString();
						row["Status"] = dr[3].ToString();
						row["Scheduled Start"] = dr[4].ToString();
						row["Scheduled End"] = dr[5].ToString();
						row["Locked Time"] = dr[1].ToString();
						row["Locked By"] = dr[6].ToString();
						row["Lock Comments"] = dr[7].ToString();
						
						row["Unlocked Time"] = dr[8].ToString();
						row["Unlocked By"] = dr[9].ToString();
						row["Unlock Comments"] = dr[10].ToString();
						
						Table.Rows.Add(row);
					}
				}
			}
		}
		
//		void InitializeGLColumns(string radioButtonText) {
//			GLColumn select = new GLColumn();
//			GLColumn Tech = new GLColumn();
//			GLColumn CellName = new GLColumn();
//			GLColumn OssId = new GLColumn();
//			GLColumn Switch = new GLColumn();
//			GLColumn Vendor = new GLColumn();
//			GLColumn NOC = new GLColumn();
//			GLColumn Locked = new GLColumn();
//
//			select.CheckBoxes = true;
//			select.Text = string.Empty;
//
//			Tech.Text = "Tech";
//			Tech.Width = 38;
//
//			CellName.Text = "Cell Name";
//
//			Switch.Text = "Switch";
//			Switch.Width = 45;
//
//			OssId.Text = "OSS ID";
//			OssId.Width = 51;
//
//			Vendor.Text = "Vendor";
//			Vendor.Width = 49;
//
//			NOC.Text = "NOC";
//			NOC.Width = 39;
//
//			Locked.Text = "Locked";
//			Locked.TextAlignment = ContentAlignment.MiddleCenter;
//			Locked.Width = 49;
//
//			if(radioButtonText == "Lock Cells")
//				glacialList1.Columns.AddRange(new [] { select, Tech, CellName, Switch, OssId, Vendor, NOC, Locked });
//			else {
//				GLColumn Reference = new GLColumn();
//				GLColumn CaseStatus = new GLColumn();
//				GLColumn CrqScheduledStart = new GLColumn();
//				GLColumn CrqScheduledEnd = new GLColumn();
//				GLColumn LockedTime = new GLColumn();
//				GLColumn LockedBy = new GLColumn();
//				GLColumn LockComments = new GLColumn();
//				Reference.Text = "Reference";
//				CaseStatus.Text = "Status";
//				CrqScheduledStart.Text = "Scheduled Start";
//				CrqScheduledEnd.Text = "Scheduled End";
//				LockedTime.Text = "Locked Time";
//				LockedBy.Text = "Locked By";
//				LockComments.Text = "Comments";
//				LockComments.Width = 100;
//				if(radioButtonText == "Unlock Cells")
//					glacialList1.Columns.AddRange(new [] { select, Tech, CellName, Switch, OssId, Vendor, NOC, Locked, Reference, CaseStatus, CrqScheduledStart, CrqScheduledEnd, LockedTime, LockedBy, LockComments });
//				else {
//					GLColumn UnlockedTime = new GLColumn();
//					GLColumn UnlockedBy = new GLColumn();
//					GLColumn UnlockComments = new GLColumn();
//					LockComments.Text = "Lock Comments";
//					UnlockComments.Width = 100;
//					UnlockComments.Text = "Unlock Comments";
//					UnlockedTime.Text = "Unlocked Time";
//					UnlockedBy.Text = "Unlocked By";
//					glacialList1.Columns.AddRange(new [] { Tech, CellName, Switch, OssId, Vendor, NOC, Reference, CaseStatus, CrqScheduledStart, CrqScheduledEnd, LockedTime, LockedBy, LockComments, UnlockedTime, UnlockedBy, UnlockComments });
//				}
//			}
//		}
		
		void GlacialList1ItemChangedEvent(object source, ChangedEventArgs e)
		{
			if(e.ChangedType == ChangedTypes.SubItemChanged && e.Column != glacialList1.Columns[0]) {
				if(e.Item.ForeColor == SystemColors.GrayText) {
					e.SubItem.Checked = false;
					return;
				}
				if(radioButton1.Checked)
					comboBox1.Enabled = glacialList1.Items.Cast<GLItem>().Where(s => s.SubItems[0].Checked).Count() > 0; // && radioButton1.Checked;
				if(radioButton2.Checked)
					amtRichTextBox1.Enabled = glacialList1.Items.Cast<GLItem>().Where(s => s.SubItems[0].Checked).Count() > 0;
				
				CheckBox cb = null;
				switch(e.Item.SubItems[1].Text) {
					case "2G":
						cb = checkBox1;
						break;
					case "3G":
						cb = checkBox2;
						break;
					case "4G":
						cb = checkBox3;
						break;
				}
				if(cb != null) {
					cb.CheckedChanged -= CheckBoxesCheckedChanged;
					cb.Checked = glacialList1.Items.Cast<GLItem>().Where(s => s.SubItems[1].Text == e.Item.SubItems[1].Text && s.SubItems[0].Checked).Count() == glacialList1.Items.Cast<GLItem>().Where(s => s.SubItems[1].Text == e.Item.SubItems[1].Text).Count();
					cb.CheckedChanged += CheckBoxesCheckedChanged;
				}
			}
		}
		
//		void GlacialList1SubItemChangedEvent(object source, ChangedEventArgs e) {
//			if(e.ChangedType == ChangedTypes.SubItemChanged) {
//				if(e.Item.ForeColor == SystemColors.GrayText)
//					e.SubItem.Checked = false;
//			}
//		}
	}
}