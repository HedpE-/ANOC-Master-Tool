/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 14-10-2016
 * Time: 01:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
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
		DataTable DataTable;
		public string DataType { get; private set; }
		
		public OiSiteTablesForm(DataTable inputDataTable, string dataToShow)
		{
			DataTable = inputDataTable;
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
			foreach(DataColumn col in DataTable.Columns)
				listView1.Columns.Add(col.ColumnName);
			foreach (DataRow row in DataTable.Rows)
				listView1.Items.Add(new ListViewItem(row.ItemArray.Cast<string>().ToArray()));
			foreach (ColumnHeader col in listView1.Columns)
				col.Width = -2;
			
			listView1.ResumeLayout();
		}
	}
}
