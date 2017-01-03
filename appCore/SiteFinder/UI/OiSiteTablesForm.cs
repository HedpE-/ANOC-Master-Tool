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
//using System.Linq;
using System.Windows.Forms;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of OiSiteTablesForm.
	/// </summary>
	public partial class OiSiteTablesForm : Form
	{
		DataTable Datatable;
//		string filter = "all";
		int maxWidth;
		public bool Cancel;
		public string DataType { get; private set; }
		public List<ListViewItem> selectedCases;
		
		public OiSiteTablesForm(DataTable inputDataTable, string dataToShow, string siteID)
		{
			Datatable = inputDataTable;
			InitializeComponent();
			listView1.Dock = DockStyle.Fill;
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
			
//			MaximumSize = new Size(maxWidth, int.MaxValue);
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
			FormClosing += OiSiteTablesFormFormClosing;
//			MaximumSize = new Size(maxWidth, int.MaxValue);
		}
		
		void populateListView() {
			listView1.SuspendLayout();
			listView1.Items.Clear();
			listView1.View = View.Details;
			listView1.CheckBoxes = DataType == "Cases";
			foreach(DataColumn col in Datatable.Columns) {
				ColumnHeader column = listView1.Columns.Add(col.ColumnName, col.ColumnName);
			}
			
			foreach (DataRow row in Datatable.Rows) {
				List<string> rowList = new List<string>();
				for(int c = 0;c < Datatable.Columns.Count;c++) {
					if(Datatable.Columns[c].DataType == typeof(DateTime)) {
						if (!(row[c] is DBNull))
							rowList.Add(Convert.ToDateTime(row[c]).ToString("dd-MM-yyyy HH:mm"));
						continue;
					}
					rowList.Add(row[c].ToString());
				}
				listView1.Items.Add(new ListViewItem(rowList.ToArray()));
			}
			
			maxWidth = 0;
			foreach (ColumnHeader col in listView1.Columns) {
				col.Width = -2;
//				maxWidth += col.Width;
			}
			
			listView1.ResumeLayout();
		}
		
		void Button1Click(object sender, EventArgs e) {
			Close();
		}
		
		void ListView1KeyDown(object sender, KeyEventArgs e) {
			if(e.Control && e.KeyCode == Keys.C) {
				if(listView1.SelectedItems.Count > 0) {
					string columnName = string.Empty;
					switch(DataType) {
						case "INCs":
							columnName = "Incident Ref";
							break;
						case "CRQs":
							columnName = "Change Ref";
							break;
						case "BookIns":
							columnName = "Engineer";
							break;
						case "ActiveAlarms":
							columnName = "Group";
							break;
					}
					try {
						Clipboard.SetText(listView1.SelectedItems[0].SubItems[listView1.Columns[columnName].Index].Text);
					}
					catch(Exception) {}
				}
			}
		}
		
		void ListView1DoubleClick(object sender, EventArgs e)
		{
			if(listView1.SelectedItems.Count > 0) {
			}
			string columnName = string.Empty;
			switch(DataType) {
				case "INCs":
					columnName = "Incident Ref";
					break;
				case "CRQs":
					columnName = "Change Ref";
					break;
				case "BookIns":
					columnName = "Engineer";
					break;
				case "ActiveAlarms":
					columnName = "Group";
					break;
			}
		}
		
		void OiSiteTablesFormFormClosing(object sender, FormClosingEventArgs e) {
			if(!Cancel) {
				if(listView1.CheckedItems.Count > 0) {
					selectedCases = new List<ListViewItem>();
					foreach(ListViewItem lvi in listView1.Items) {
						if(lvi.Checked)
							selectedCases.Add(lvi);
					}
				}
				else {
					DialogResult ans = appCore.UI.FlexibleMessageBox.Show("You didn't select any cases to relate, is this right?", "No cases selected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if(ans == DialogResult.No)
						e.Cancel = true;
					else
						selectedCases = new List<ListViewItem>();
				}
			}
		}
		
		void CheckBox1CheckedChanged(object sender, EventArgs e) {
			if(checkBox1.Enabled)
				button1.Enabled = checkBox1.Checked;
		}
		
		void ListView1ItemChecked(object sender, ItemCheckedEventArgs e) {
			checkBox1.Enabled = listView1.CheckedItems.Count == 0;
			button1.Enabled = listView1.CheckedItems.Count > 0;
		}
		
		void CheckBox1EnabledChanged(object sender, EventArgs e) {
			if(!checkBox1.Enabled)
				checkBox1.Checked = false;
		}
		
		void Button2Click(object sender, EventArgs e) {
			Cancel = true;
			Close();
		}
	}
}
