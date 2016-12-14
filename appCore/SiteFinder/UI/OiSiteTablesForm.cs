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
using System.Linq;
using System.Windows.Forms;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of OiSiteTablesForm.
	/// </summary>
	public partial class OiSiteTablesForm : Form
	{
		DataTable Datatable;
		string filter = "all";
		public string DataType { get; private set; }
		int maxWidth;
		public List<ListViewItem> selectedCases;
		
		public OiSiteTablesForm(DataTable inputDataTable, string dataToShow, string siteID)
		{
			Datatable = inputDataTable;
			InitializeComponent();
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
			if(Datatable.TableName == "table_inc") {
				if(filter == "all") {
					DataRow[] filteredRows = Datatable.Select("Status NOT LIKE 'Closed' AND Status NOT LIKE 'Resolved'");
					filter = "filtered";
				}
				else {
					populateListView();
					filter = "all";
				}
			}
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
			if(listView1.CheckedItems.Count > 0)
				selectedCases = listView1.Items.Cast<ListViewItem>().Where(s => s.Checked).ToList();
			else {
				DialogResult ans = appCore.UI.FlexibleMessageBox.Show("You didn't select any cases to relate, is this right?", "No cases selected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if(ans == DialogResult.No)
					e.Cancel = true;
				else
					selectedCases = new List<ListViewItem>();
			}
		}
	}
}
