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
		DataTable Table;
		
		int checkedCount {
			get {
				int c = 0;
				foreach(DataGridViewRow row in dataGridView1.Rows) {
					if(row.Cells[0].Value != null) {
						if(Convert.ToBoolean(row.Cells[0].Value))
							c++;
					}
				}
				return c;
			}
		}
		
		int gsmCellsCount {
			get {
				int c = 0;
				foreach(DataGridViewRow row in dataGridView1.Rows) {
					if(row.Cells["Tech"].Value != null) {
						if(row.Cells["Tech"].Value.ToString() == "2G" && !row.Frozen)
							c++;
					}
				}
				return c;
			}
		}
		
		int gsmCheckedCount {
			get {
				int c = 0;
				foreach(DataGridViewRow row in dataGridView1.Rows) {
					if(row.Cells[0].Value != null) {
						if(Convert.ToBoolean(row.Cells[0].Value) && row.Cells["Tech"].Value.ToString() == "2G")
							c++;
					}
				}
				return c;
			}
		}
		
		int umtsCellsCount {
			get {
				int c = 0;
				foreach(DataGridViewRow row in dataGridView1.Rows) {
					if(row.Cells["Tech"].Value != null) {
						if(row.Cells["Tech"].Value.ToString() == "3G" && !row.Frozen)
							c++;
					}
				}
				return c;
			}
		}
		
		int umtsCheckedCount {
			get {
				int c = 0;
				foreach(DataGridViewRow row in dataGridView1.Rows) {
					if(row.Cells[0].Value != null) {
						if(Convert.ToBoolean(row.Cells[0].Value) && row.Cells["Tech"].Value.ToString() == "3G")
							c++;
					}
				}
				return c;
			}
		}
		
		int lteCellsCount {
			get {
				int c = 0;
				foreach(DataGridViewRow row in dataGridView1.Rows) {
					if(row.Cells["Tech"].Value != null) {
						if(row.Cells["Tech"].Value.ToString() == "4G" && !row.Frozen)
							c++;
					}
				}
				return c;
			}
		}
		
		int lteCheckedCount {
			get {
				int c = 0;
				foreach(DataGridViewRow row in dataGridView1.Rows) {
					if(row.Cells[0].Value != null) {
						if(Convert.ToBoolean(row.Cells[0].Value) && row.Cells["Tech"].Value.ToString() == "4G")
							c++;
					}
				}
				return c;
			}
		}
		
		string uiMode;
		string UiMode {
			get { return uiMode; }
			set {
				uiMode = value;
				switch(uiMode) {
					case "Lock Cells":
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
						dataGridView1.Width = 795;
						dataGridView1.Columns["Locked"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
						break;
					case "Unlock Cells":
						button1.Text = "Unlock\nCells";
						checkBox1.Enabled = currentSite.Cells.Filter(Cell.Filters.All_2G).Where(s => s.Locked).Count() > 0;
						checkBox2.Enabled = currentSite.Cells.Filter(Cell.Filters.All_3G).Where(s => s.Locked).Count() > 0;
						checkBox3.Enabled = currentSite.Cells.Filter(Cell.Filters.All_4G).Where(s => s.Locked).Count() > 0;
						dataGridView1.Width = 795;
						dataGridView1.Columns["Lock Comments"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns["Lock Comments"].Width = 300;
						dataGridView1.Columns["Lock Comments"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
						dataGridView1.Columns["Locked"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
						break;
					case "History":
						dataGridView1.Width = dataGridView1.Right + (amtRichTextBox1.Right - dataGridView1.Right) - 5;
						dataGridView1.Columns["Lock Comments"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns["Lock Comments"].Width = 300;
						dataGridView1.Columns["Lock Comments"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
						dataGridView1.Columns["Unlock Comments"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns["Unlock Comments"].Width = 300;
						dataGridView1.Columns["Unlock Comments"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
						break;
				}
				
				checkBox1.Checked =
					checkBox2.Checked =
					checkBox3.Checked = false;
				amtRichTextBox1.Text =
					comboBox1.Text = string.Empty;
				
				foreach(Control ctrl in Controls) {
					switch(ctrl.GetType().ToString()) {
						case "System.Windows.Forms.CheckBox":
						case "System.Windows.Forms.Label":
						case "System.Windows.Forms.ComboBox":
						case "System.Windows.Forms.Button":
						case "appCore.UI.AMTRichTextBox":
							if(ctrl.Name != "label4")
								ctrl.Visible = uiMode.Contains("ock Cells");
							break;
					}
				}
				checkBookIn();
			}
		}
		
		public LockUnlockCellsForm(Site site) {
			InitializeComponent();
			
			currentSite = site;
			
			Text = "Site " + currentSite.Id + " Lock/Unlock cells";
			
			currentSite.requestOIData("LKULK");
//			currentSite.requestOIData("INCCRQ");
			
			radioButton1.Select();
		}
		
		void RadioButtonsCheckedChanged(object sender, EventArgs e) {
			RadioButton rb = sender as RadioButton;
			
			if(rb.Checked) {
				SuspendLayout();
				
				dataGridView1.DataSource = null;
				dataGridView1.Columns.Clear();
				
				InitializeDataTable(rb.Text);
				
				uiMode = rb.Text;
				
				dataGridView1.DataSource = Table;
				
				UiMode = rb.Text;
				
				ResumeLayout();
			}
		}
		
		void checkBookIn() {
			if(UiMode == "Lock Cells") {
				if(currentSite.BookIns == null)
					currentSite.requestOIData("Bookins");
				
				var foundBookIns = currentSite.BookIns.Rows.Cast<DataRow>().Where(s => string.IsNullOrEmpty(s[11].ToString()));
				if(foundBookIns.Count() > 0) {
					DataRow bookInRow = null;
					foreach(DataRow bookIn in foundBookIns) {
						DateTime arrivedTime = Convert.ToDateTime(bookIn[7].ToString());
						if(arrivedTime.Year == DateTime.Now.Year && arrivedTime.Month == DateTime.Now.Month && arrivedTime.Day == DateTime.Now.Day) {
							bookInRow = bookIn;
							break;
						}
					}
					if(bookInRow != null) {
						label4.Visible = true;
						label4.Text = "Valid Book In found: " + bookInRow[3] + " - " + bookInRow[4] + " - " + bookInRow[5] + " - " + bookInRow[7];
						label4.ForeColor = Color.DarkGreen;
						label4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
					}
					else {
						label4.Visible = true;
						label4.Text = "CAUTION!! No valid Book In found";
						label4.ForeColor = Color.Red;
						label4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
					}
				}
				else {
					label4.Visible = true;
					label4.Text = "CAUTION!! No valid Book In found";
					label4.ForeColor = Color.Red;
					label4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
				}
			}
			else
				label4.Visible = false;
		}
		
		void CheckBoxesCheckedChanged(object sender, EventArgs e) {
			CheckBox cb = sender as CheckBox;
			var filtered = dataGridView1.Rows.Cast<DataGridViewRow>().Where(s => s.Cells["Tech"].Value.ToString() == cb.Text);
			
			foreach(DataGridViewRow dgvr in filtered) {
				if(dgvr.Cells[0].Style.ForeColor != SystemColors.GrayText) {
					DataGridViewCheckBoxCell cell = dgvr.Cells[0] as DataGridViewCheckBoxCell;
					dataGridView1.CellValueChanged -= DataGridView1CellValueChanged;
					
					if(filtered.Last() == dgvr) {
						dataGridView1.CellValueChanged += DataGridView1CellValueChanged;
						cell.Value = cb.Checked ? cell.TrueValue : cell.FalseValue;
					}
					else {
						cell.Value = cb.Checked ? cell.TrueValue : cell.FalseValue;
						dataGridView1.CellValueChanged += DataGridView1CellValueChanged;
					}
				}
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
			DialogResult ans = DialogResult.Yes;
			if(label4.Visible && label4.Text.StartsWith("CAUTION"))
				ans = appCore.UI.FlexibleMessageBox.Show("No valid book in found for this site.\n\nContinue anyway?","No book in", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if(ans == DialogResult.Yes) {
				var filtered = dataGridView1.Rows.Cast<DataGridViewRow>().Where(s => (bool?)s.Cells[0].Value == true);
				List<string> cellsList = new List<string>();
				foreach(DataGridViewRow row in filtered)
					cellsList.Add(row.Cells["Cell Name"].Value.ToString());
				if(UiMode.StartsWith("Lock"))
					sendLockCellsRequest(cellsList, comboBox1.Text, amtRichTextBox1.Text);
				else
					sendUnlockCellsRequest(cellsList, amtRichTextBox1.Text);
			}
		}
		
		void sendLockCellsRequest(List<string> cellsList, string reference, string comments) {
			bool manRef = !comboBox1.Items.Contains(reference);
			OIConnection.requestPhpOutput("enterlock", currentSite.Id, cellsList, reference, comments, manRef);
			currentSite.requestOIData("LKULK", true);
			RadioButtonsCheckedChanged(radioButton1, null);
		}
		
		void sendUnlockCellsRequest(List<string> cellsList, string comments) {
			OIConnection.requestPhpOutput("cellslocked", currentSite.Id, cellsList, comments);
			currentSite.requestOIData("LKULK", true);
			RadioButtonsCheckedChanged(radioButton2, null);
		}
		
		void InitializeDataTable(string radioButtonText) {
			Table = new DataTable();
			
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
				addCheckBoxColumn();
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
							currentSite.requestOIData("LKULK", true);
						
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
					addCheckBoxColumn();
				}
				else {
					DataColumn UnlockedTime = new DataColumn("Unlocked Time");
					DataColumn UnlockedBy = new DataColumn("Unlocked By");
					DataColumn UnlockComments = new DataColumn("Unlock Comments");
					Table.Columns.AddRange(new [] { UnlockedTime, UnlockedBy, UnlockComments });
					
					if(currentSite.LockedCellsDetails == null)
						currentSite.requestOIData("LKULK", true);
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
		
		DataGridViewCheckBoxColumn addCheckBoxColumn() {
			DataGridViewCheckBoxColumn chkColumn = new DataGridViewCheckBoxColumn();
			chkColumn.HeaderText = "";
			chkColumn.ThreeState = false;
			chkColumn.FalseValue = false;
			chkColumn.TrueValue = true;
			chkColumn.Width = 19;
			dataGridView1.Columns.Insert(0, chkColumn);
			return chkColumn;
		}
		
		void DataGridView1CellContentClick(object sender, DataGridViewCellEventArgs e) {
			if(e.ColumnIndex == 0) {
				if(!dataGridView1.Rows[e.RowIndex].Frozen) {
					DataGridViewCheckBoxCell cell = dataGridView1.Rows[e.RowIndex].Cells[0] as DataGridViewCheckBoxCell;
					cell.Value = cell.Value != null ? !Convert.ToBoolean(cell.Value) : cell.TrueValue;
				}
			}
//			else {
//				if(e.Item.ForeColor == SystemColors.GrayText) {
//					e.SubItem.Checked = false;
//					return;
//				}
//			}
		}
		
		void DataGridView1CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if(e.ColumnIndex == 0) {
				if(radioButton1.Checked)
					comboBox1.Enabled = checkedCount > 0; // && radioButton1.Checked;
				if(radioButton2.Checked)
					amtRichTextBox1.Enabled = checkedCount > 0;
				
				CheckBox cb = null;
				int checkCount = 0;
				int maxCount = 0;
				switch(dataGridView1.Rows[e.RowIndex].Cells["Tech"].Value.ToString()) {
					case "2G":
						cb = checkBox1;
						checkCount = gsmCheckedCount;
						maxCount = gsmCellsCount;
						break;
					case "3G":
						cb = checkBox2;
						checkCount = umtsCheckedCount;
						maxCount = umtsCellsCount;
						break;
					case "4G":
						cb = checkBox3;
						checkCount = lteCheckedCount;
						maxCount = lteCellsCount;
						break;
				}
				if(cb != null) {
					cb.CheckedChanged -= CheckBoxesCheckedChanged;
					cb.Checked = checkCount == maxCount;
					cb.CheckedChanged += CheckBoxesCheckedChanged;
				}
			}
		}
		
		void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
			if(!dataGridView1.Rows[e.RowIndex].Cells["NOC"].Value.ToString().Contains("ANOC")) {
				e.CellStyle.ForeColor = SystemColors.GrayText;
				if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
					e.CellStyle.BackColor = SystemColors.InactiveBorder;
				dataGridView1.Rows[e.RowIndex].Frozen = true;
			}
			else {
				switch(UiMode) {
					case "Lock Cells":
						if(dataGridView1.Rows[e.RowIndex].Cells["Locked"].Value.ToString() == "YES") {
							e.CellStyle.ForeColor = SystemColors.GrayText;
							if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
								e.CellStyle.BackColor = SystemColors.InactiveBorder;
							dataGridView1.Rows[e.RowIndex].Frozen = true;
						}
						else {
							e.CellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
							if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
								e.CellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
							dataGridView1.Rows[e.RowIndex].Frozen = false;
						}
						break;
					case "Unlock Cells":
						if(dataGridView1.Rows[e.RowIndex].Cells["Locked"].Value.ToString() == "No") {
							e.CellStyle.ForeColor = SystemColors.GrayText;
							if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
								e.CellStyle.BackColor = SystemColors.InactiveBorder;
							dataGridView1.Rows[e.RowIndex].Frozen = true;
						}
						else {
							e.CellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
							if(dataGridView1.Columns[e.ColumnIndex].Name != "Locked")
								e.CellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
							dataGridView1.Rows[e.RowIndex].Frozen = false;
						}
						break;
//					case "History":
//						e.CellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
//						e.CellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
//						break;
				}
			}
			if(dataGridView1.Columns[e.ColumnIndex].Name == "Locked")
				e.CellStyle.BackColor = e.Value.ToString() == "YES" ? Color.OrangeRed : Color.LightGreen;
		}
	}
}