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
using appCore.SiteFinder;

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
		
		public OiSiteTablesForm(DataTable inputDataTable, string dataToShow)
		{
			Datatable = inputDataTable;
			InitializeComponent();
			switch(dataToShow) {
				case "INC":
					this.Name = "INCsOiDataTableForm";
					DataType = "INCs";
					break;
				case "CRQ":
					this.Name = "CRQsOiDataTableForm";
					DataType = "CRQs";
					break;
				case "Alarms":
					this.Name = "ActiveAlarmsOiDataTableForm";
					DataType = "ActiveAlarms";
					break;
				case "BookIns":
					this.Name = "BookInsOiDataTableForm";
					DataType = "BookIns";
					break;
			}
			populateListView();
			this.Text = DataType;
		}
		
		void populateListView() {
			listView1.SuspendLayout();
			listView1.Items.Clear();
			listView1.View = View.Details;
			foreach(DataColumn col in Datatable.Columns) {
				listView1.Columns.Add(col.ColumnName);
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
			
			foreach (ColumnHeader col in listView1.Columns)
				col.Width = -2;
			
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
	}
}
