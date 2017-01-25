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
		
		public OiSiteTablesForm(DataTable inputDataTable, string dataToShow, string siteID)
		{
			Datatable = inputDataTable;
			InitializeComponent();
			dataGridView1.Dock = DockStyle.Fill;
			switch(dataToShow) {
				case "INCs":
					Name = "INCsOiDataTableForm";
					DataType = "INCs";
					break;
				case "CRQs":
					Name = "CRQsOiDataTableForm";
					DataType = "CRQs";
					break;
				case "ActiveAlarms":
					Name = "ActiveAlarmsOiDataTableForm";
					DataType = "ActiveAlarms";
					break;
				case "BookIns":
					Name = "BookInsOiDataTableForm";
					DataType = "BookIns";
					break;
			}
			populateListView();
			Text = "Site " + siteID + " " + DataType;
		}
		
		public OiSiteTablesForm(DataTable currentCases, string siteID)
		{
			Datatable = currentCases;
			InitializeComponent();
			ControlBox = false;
			checkBox1.Visible = button1.Visible = button2.Visible = true;
			DataType = "Cases";
			populateListView();
			Text = "Select related cases - Site " + siteID;
			if(dataGridView1.Right < 900)
				Width = dataGridView1.Right;
			FormClosing += OiSiteTablesFormFormClosing;
//			MaximumSize = new Size(maxWidth, int.MaxValue);
		}
		
		void populateListView() {
			dataGridView1.SuspendLayout();
			
			dataGridView1.DataSource = Datatable;
			
			try {
				dataGridView1.Columns["Resolution"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
				dataGridView1.Columns["Resolution"].Width = 300;
				dataGridView1.Columns["Resolution"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
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
				if(cell.Value != null)
					cell.Value = !Convert.ToBoolean(cell.Value);
				else
					cell.Value = cell.TrueValue;
				
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
	}
}
