/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 14-10-2016
 * Time: 01:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using appCore.OI.JSON;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of OiSiteTablesForm.
	/// </summary>
	public partial class OiSiteTablesForm : Form
	{
		DataTable Datatable;
		List<Incident> Incidents;
		List<Change> Changes;
		List<BookIn> Visits;
		List<Alarm> Alarms;
		int maxWidth;
		public bool Cancel;
		string DataType { get; set; }
		public List<DataGridViewRow> selectedCases;
		public Control OwnerControl {
			get;
			private set;
		}
		
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
		
		public OiSiteTablesForm(List<Incident> inputData, string siteID, Control owner = null) {
			if(owner != null)
				OwnerControl = owner;
			Incidents = inputData;
			InitializeComponent();
			dataGridView1.Dock = DockStyle.Fill;
			
			Name = "INCsOiTableForm";
			DataType = "INCs";
			dataGridView1.CellFormatting += dataGridView1_CellFormatting;
			
			populateGridView();
			Text = "Site " + siteID + " " + DataType;
		}
		
		public OiSiteTablesForm(List<Change> inputData, string siteID, Control owner = null) {
			if(owner != null)
				OwnerControl = owner;
			Changes = inputData;
			InitializeComponent();
			dataGridView1.Dock = DockStyle.Fill;
			
			Name = "CRQsOiDataTableForm";
			DataType = "CRQs";
			dataGridView1.CellFormatting += dataGridView1_CellFormatting;
			
			populateGridView();
			Text = "Site " + siteID + " " + DataType;
		}
		
		public OiSiteTablesForm(List<BookIn> inputData, string siteID, Control owner = null) {
			if(owner != null)
				OwnerControl = owner;
			Visits = inputData;
			InitializeComponent();
			dataGridView1.Dock = DockStyle.Fill;
			
			Name = "BookInsOiDataTableForm";
			DataType = "BookIns";
			dataGridView1.CellFormatting += dataGridView1_CellFormatting;
			
			populateGridView();
			Text = "Site " + siteID + " " + DataType;
		}
		
		public OiSiteTablesForm(List<Alarm> inputData, string siteID, Control owner = null) {
			if(owner != null)
				OwnerControl = owner;
			Alarms = inputData;
			InitializeComponent();
			dataGridView1.Dock = DockStyle.Fill;
			
			Name = "ActiveAlarmsOiDataTableForm";
			DataType = "ActiveAlarms";
			
			populateGridView();
			Text = "Site " + siteID + " " + DataType;
		}
		
		public OiSiteTablesForm(DataTable inputDataTable, string dataToShow, string siteID, Control owner = null) {
			if(owner != null)
				OwnerControl = owner;
			Datatable = inputDataTable;
			InitializeComponent();
			dataGridView1.Dock = DockStyle.Fill;
			
			Name = "AvailabilityDataTableForm";
			DataType = dataToShow;
			dataGridView1.CellFormatting += dataGridView1_CellFormatting;
			
			populateGridView();
			Text = "Site " + siteID + " " + DataType;
		}
		
		public OiSiteTablesForm(DataTable currentCases, string siteID, Control owner = null) {
			if(owner != null)
				OwnerControl = owner;
			Datatable = currentCases;
			InitializeComponent();
			ControlBox = false;
			checkBox1.Visible = button1.Visible = button2.Visible = true;
			DataType = "Cases";
			populateGridView();
			Text = "Select related cases - Site " + siteID;
			if(dataGridView1.Right < 900)
				Width = dataGridView1.Right;
			FormClosing += OiSiteTablesFormFormClosing;
//			MaximumSize = new Size(maxWidth, int.MaxValue);
		}
		
		void populateGridView() {
			dataGridView1.SuspendLayout();
			
			switch(DataType) {
				case "INCs":
					dataGridView1.DataSource = Incidents;
					break;
				case "CRQs":
					dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
					dataGridView1.DataSource = Changes;
					dataGridView1.RowTemplate.Height = 30;
					break;
				case "ActiveAlarms":
					dataGridView1.DataSource = Alarms;
					break;
				case "BookIns":
					dataGridView1.DataSource = Visits;
					try { dataGridView1.Sort(dataGridView1.Columns["Arrived"], System.ComponentModel.ListSortDirection.Descending); } catch { }
					break;
				case "Availability":
					dataGridView1.DataSource = Datatable;
					dataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
					foreach(DataGridViewColumn dgvc in dataGridView1.Columns) {
						if(dgvc.Name.Contains(" ")) {
							dgvc.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
							dgvc.Width = 40;
						}
						else
							dgvc.Frozen = true;
					}
//					dataGridView1.FirstDisplayedScrollingColumnIndex = dataGridView1.Columns.Count - 1;
					break;
				case "Cases":
					dataGridView1.DataSource = Datatable;
					DataGridViewCheckBoxColumn chkColumn = new DataGridViewCheckBoxColumn();
					chkColumn.HeaderText = "";
					chkColumn.ThreeState = false;
					chkColumn.FalseValue = false;
					chkColumn.TrueValue = true;
					chkColumn.Width = 19;
					dataGridView1.Columns.Insert(0, chkColumn);
					dataGridView1.CellContentClick += DataGridView1CellContentClick;
					break;
			}
			
			foreach(DataGridViewColumn col in dataGridView1.Columns) {
				switch(col.Name) {
					case "Resolution":
						dataGridView1.Columns["Resolution"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns["Resolution"].Width = 300;
						dataGridView1.Columns["Resolution"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
						break;
					case "Programme":
						dataGridView1.Columns["Programme"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
						dataGridView1.Columns["Programme"].Width = 300;
						dataGridView1.Columns["Programme"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
						break;
					default:
						col.Name = col.HeaderText = col.Name.Replace('_', ' ');
						break;
				}
			}
			
			dataGridView1.ResumeLayout();
			
//			foreach(DataGridViewRow row in dataGridView1.Rows) {
//				if(row.Height > 10) {
//					dataGridView1.AutoResizeRow(row.Index, DataGridViewAutoSizeRowMode.RowHeader);
//					row.Height = 10;
//				}
//			}
		}
		
		void Button1Click(object sender, EventArgs e) {
			Close();
		}
		
		void OiSiteTablesFormFormClosing(object sender, FormClosingEventArgs e) {
			if(!Cancel) {
				if(checkedCount > 0) {
					selectedCases = new List<DataGridViewRow>();
					foreach(DataGridViewRow row in dataGridView1.Rows) {
						if(row.Cells[0].Value != null) {
							if(Convert.ToBoolean(row.Cells[0].Value))
								selectedCases.Add(row);
						}
					}
				}
				else {
					DialogResult ans = appCore.UI.FlexibleMessageBox.Show("You didn't select any cases to relate, is this right?", "No cases selected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if(ans == DialogResult.No)
						e.Cancel = true;
					else
						selectedCases = new List<DataGridViewRow>();
				}
			}
		}
		
		void CheckBox1CheckedChanged(object sender, EventArgs e) {
			if(checkBox1.Enabled)
				button1.Enabled = checkBox1.Checked;
		}
		
		void CheckBox1EnabledChanged(object sender, EventArgs e) {
			if(!checkBox1.Enabled)
				checkBox1.Checked = false;
		}
		
		void Button2Click(object sender, EventArgs e) {
			Cancel = true;
			Close();
		}
		
		void DataGridView1CellContentClick(object sender, DataGridViewCellEventArgs e) {
			if(e.ColumnIndex == 0) {
				DataGridViewCheckBoxCell cell = dataGridView1.Rows[e.RowIndex].Cells[0] as DataGridViewCheckBoxCell;
				cell.Value = cell.Value != null ? !Convert.ToBoolean(cell.Value) : cell.TrueValue;
				
				if(cell.Value == cell.TrueValue) {
					checkBox1.Enabled = false;
					button1.Enabled = true;
				}
				else {
					checkBox1.Enabled = checkedCount == 0;
					button1.Enabled = checkedCount > 0;
				}
			}
		}
		
		void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) {
			switch(DataType) {
				case "INCs":
					if(dataGridView1.Columns[e.ColumnIndex].Name == "Status" || dataGridView1.Columns[e.ColumnIndex].Name == "Incident Ref") {
						if(dataGridView1.Rows[e.RowIndex].Cells["Status"].Value.ToString() != "Closed" && dataGridView1.Rows[e.RowIndex].Cells["Status"].Value.ToString() != "Resolved")
							e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
						else {
							if(dataGridView1.Rows[e.RowIndex].Cells["Status"].Value.ToString() == "Resolved")
								e.CellStyle.BackColor = System.Drawing.Color.Yellow;
						}
					}
					break;
				case "CRQs":
					if(dataGridView1.Columns[e.ColumnIndex].Name == "Status" || dataGridView1.Columns[e.ColumnIndex].Name == "Change Ref") {
						if(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Scheduled" || dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Implementation In Progress") {
							if(Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["Scheduled Start"].Value) <= DateTime.Now &&
							   Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["Scheduled End"].Value) > DateTime.Now)
								e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
							else
								e.CellStyle.BackColor = System.Drawing.Color.Yellow;
						}
						else {
							if(dataGridView1.Rows[e.RowIndex].Cells["Status"].Value.ToString() != "Closed")
								e.CellStyle.BackColor = System.Drawing.Color.Red;
						}
					}
					break;
				case "BookIns":
					if(dataGridView1.Columns[e.ColumnIndex].Name == "Arrived" || dataGridView1.Columns[e.ColumnIndex].Name == "Visit") {
						if(Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["Arrived"].Value) <= DateTime.Now &&
						   dataGridView1.Rows[e.RowIndex].Cells["Departed Site"].Value == null)
							e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
					}
					break;
				case "Availability":
					if(dataGridView1.Columns[e.ColumnIndex].Name.Contains(" ")) {
						string cellValue = e.Value.ToString();
						if(!string.IsNullOrEmpty(cellValue)) {
							int cellIntValue = Convert.ToInt16(cellValue);
							if(cellIntValue < 11)
								e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
							else
								e.CellStyle.BackColor = cellIntValue < 3000 ?
									System.Drawing.Color.Yellow :
									System.Drawing.Color.Red;
						}
						else
							e.CellStyle.BackColor = System.Drawing.Color.DimGray;
					}
					break;
			}
		}
	}
}