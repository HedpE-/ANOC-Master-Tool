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
using System.Windows.Forms;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of OiSiteTablesForm.
	/// </summary>
	public partial class OiSiteTablesForm : Form
	{
		DataTable Datatable;
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
		
		public OiSiteTablesForm(DataTable inputDataTable, string dataToShow, string siteID, Control owner = null) {
			if(owner != null)
				OwnerControl = owner;
			Datatable = inputDataTable;
			InitializeComponent();
			dataGridView1.Dock = DockStyle.Fill;
			switch(dataToShow) {
				case "INCs":
					Name = "INCsOiDataTableForm";
					DataType = "INCs";
					dataGridView1.CellFormatting += dataGridView1_CellFormatting;
					break;
				case "CRQs":
					Name = "CRQsOiDataTableForm";
					DataType = "CRQs";
					dataGridView1.CellFormatting += dataGridView1_CellFormatting;
					break;
				case "ActiveAlarms":
					Name = "ActiveAlarmsOiDataTableForm";
					DataType = "ActiveAlarms";
					break;
				case "BookIns":
					Name = "BookInsOiDataTableForm";
					DataType = "BookIns";
					dataGridView1.CellFormatting += dataGridView1_CellFormatting;
					break;
				case "Availability":
					Name = "AvailabilityDataTableForm";
					DataType = "Availability Chart";
//					dataGridView1.CellFormatting += dataGridView1_CellFormatting;
					break;
			}
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
			
			dataGridView1.DataSource = Datatable;
			
			try {
				dataGridView1.Columns["Resolution"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
				dataGridView1.Columns["Resolution"].Width = 300;
				dataGridView1.Columns["Resolution"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
			} catch {}
			try {
				dataGridView1.Columns["Programme"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
				dataGridView1.Columns["Programme"].Width = 300;
				dataGridView1.Columns["Programme"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
			} catch {}
			try {
				dataGridView1.Sort(dataGridView1.Columns["Arrived"], System.ComponentModel.ListSortDirection.Descending);;
			} catch {}
			
			if(DataType == "Cases") {
				DataGridViewCheckBoxColumn chkColumn = new DataGridViewCheckBoxColumn();
				chkColumn.HeaderText = "";
				chkColumn.ThreeState = false;
				chkColumn.FalseValue = false;
				chkColumn.TrueValue = true;
				chkColumn.Width = 19;
				dataGridView1.Columns.Insert(0, chkColumn);
				dataGridView1.CellContentClick += DataGridView1CellContentClick;
			}
			
			dataGridView1.ResumeLayout();
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
			if(dataGridView1.Columns[e.ColumnIndex].Name == "Status") {
				switch(DataType) {
					case "INCs":
						if(e.Value.ToString() != "Closed" && e.Value.ToString() != "Resolved")
							e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
						else {
							if(e.Value.ToString() == "Resolved")
								e.CellStyle.BackColor = System.Drawing.Color.Yellow;
						}
						break;
					case "CRQs":
						if(e.Value.ToString() == "Scheduled" || e.Value.ToString() == "Implementation In Progress") {
							if(Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["Scheduled Start"].Value) <= DateTime.Now &&
							   Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["Scheduled End"].Value) > DateTime.Now)
								e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
							else
								e.CellStyle.BackColor = System.Drawing.Color.Yellow;
						}
						else {
							if(e.Value.ToString() != "Closed")
								e.CellStyle.BackColor = System.Drawing.Color.Red;
						}
						break;
				}
			}
			if(dataGridView1.Columns[e.ColumnIndex].Name == "Arrived") {
				if(Convert.ToDateTime(e.Value) <= DateTime.Now &&
				   string.IsNullOrEmpty(dataGridView1.Rows[e.RowIndex].Cells["Departed Site"].Value.ToString()))
					e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
			}
		}
	}
}
