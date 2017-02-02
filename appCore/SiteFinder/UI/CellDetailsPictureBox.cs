/*
 * Created by SharpDevelop.
 * User: Caramelos
 * Date: 30-10-2016
 * Time: 03:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace appCore.SiteFinder.UI
{
	/// <summary>
	/// Description of CellDetailsPictureBox.
	/// </summary>
	public class CellDetailsPictureBox : PictureBox
	{
		Label label_VF_2GCells = new Label();
		Label label_VF_3GCells = new Label();
		Label label_VF_4GCells = new Label();
		Label label_TF_4GCells = new Label();
		Label label_TF_3GCells = new Label();
		Label label_TF_2GCells = new Label();
		Label label_Total_4GCells = new Label();
		Label label_Total_3GCells = new Label();
		Label label_Total_2GCells = new Label();
		Label label_TotalCells = new Label();
		
		List<Cell> Cells;
		
		public CellDetailsPictureBox() {
			InitializeComponent();
		}
		
		public void UpdateCells(List<Cell> cells) {
			Cells = cells;
			if(Cells != null) {
				if(Cells.Count > 0) {
					// Fill Cell Count table
					
					label_TotalCells.Text = Cells.Count.ToString();
					
//				cellsList.RowFilter = "BEARER = '2G'";
					label_Total_2GCells.Text = Cells.Filter(Cell.Filters.All_2G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '2G' AND (CELL_NAME NOT LIKE 'T*' AND CELL_NAME NOT LIKE '*W' AND CELL_NAME NOT LIKE '*X' AND CELL_NAME NOT LIKE '*Y')";
					label_VF_2GCells.Text = Cells.Filter(Cell.Filters.VF_2G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '2G' AND (CELL_NAME LIKE 'T*' OR CELL_NAME LIKE '*W' OR CELL_NAME LIKE '*X' OR CELL_NAME LIKE '*Y')";
					label_TF_2GCells.Text = Cells.Filter(Cell.Filters.TF_2G).Count.ToString();
					
//				cells.RowFilter = "BEARER = '3G'";
					label_Total_3GCells.Text = Cells.Filter(Cell.Filters.All_3G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '3G' AND CELL_NAME NOT LIKE 'T*'";
					label_VF_3GCells.Text = Cells.Filter(Cell.Filters.VF_3G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '3G' AND CELL_NAME LIKE 'T*'";
					label_TF_3GCells.Text = Cells.Filter(Cell.Filters.TF_3G).Count.ToString();
					
//				cells.RowFilter = "BEARER = '4G'";
					label_Total_4GCells.Text = Cells.Filter(Cell.Filters.All_4G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '4G' AND CELL_NAME NOT LIKE 'T*'";
					label_VF_4GCells.Text = Cells.Filter(Cell.Filters.VF_4G).Count.ToString();
//				cellsList.RowFilter = "BEARER = '4G' AND CELL_NAME LIKE 'T*'";
					label_TF_4GCells.Text = Cells.Filter(Cell.Filters.TF_4G).Count.ToString();
				}
				else {
					foreach(Control ctr in Controls) {
						if(ctr.Name.StartsWith("label_"))
							ctr.Text = "0";
					}
				}
			}
			else {
				foreach(Control ctr in Controls) {
					if(ctr.Name.StartsWith("label_"))
						ctr.Text = "0";
				}
			}
		}
		
		public void UpdateCells(DataView cells) {
			// Fill Cell Count table
			
			label_TotalCells.Text = cells.Count.ToString();
			
			cells.RowFilter = "BEARER = '2G'";
			label_Total_2GCells.Text = cells.Count.ToString();
			cells.RowFilter = "BEARER = '2G' AND (CELL_NAME NOT LIKE 'T*' AND CELL_NAME NOT LIKE '*W' AND CELL_NAME NOT LIKE '*X' AND CELL_NAME NOT LIKE '*Y')";
			label_VF_2GCells.Text = cells.Count.ToString();
			cells.RowFilter = "BEARER = '2G' AND (CELL_NAME LIKE 'T*' OR CELL_NAME LIKE '*W' OR CELL_NAME LIKE '*X' OR CELL_NAME LIKE '*Y')";
			label_TF_2GCells.Text = cells.Count.ToString();
			
			cells.RowFilter = "BEARER = '3G'";
			label_Total_3GCells.Text = cells.Count.ToString();
			cells.RowFilter = "BEARER = '3G' AND CELL_NAME NOT LIKE 'T*'";
			label_VF_3GCells.Text = cells.Count.ToString();
			cells.RowFilter = "BEARER = '3G' AND CELL_NAME LIKE 'T*'";
			label_TF_3GCells.Text = cells.Count.ToString();
			
			cells.RowFilter = "BEARER = '4G'";
			label_Total_4GCells.Text = cells.Count.ToString();
			cells.RowFilter = "BEARER = '4G' AND CELL_NAME NOT LIKE 'T*'";
			label_VF_4GCells.Text = cells.Count.ToString();
			cells.RowFilter = "BEARER = '4G' AND CELL_NAME LIKE 'T*'";
			label_TF_4GCells.Text = cells.Count.ToString();
		}
		
		void InitializeComponent() {
			label_VF_2GCells = new Label();
			label_VF_3GCells = new Label();
			label_VF_4GCells = new Label();
			label_TF_4GCells = new Label();
			label_TF_3GCells = new Label();
			label_TF_2GCells = new Label();
			label_Total_4GCells = new Label();
			label_Total_3GCells = new Label();
			label_Total_2GCells = new Label();
			label_TotalCells = new Label();
			// 
			// CellDetailsPictureBox
			// 
			Image = appCore.UI.Resources.Cells_Totals;
			Name = "CellDetailsPictureBox";
			Size = new Size(403, 75);
			// 
			// label_VF_2GCells
			// 
			label_VF_2GCells.BackColor = Color.DimGray;
			label_VF_2GCells.FlatStyle = FlatStyle.Flat;
			label_VF_2GCells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			label_VF_2GCells.ForeColor = SystemColors.ControlLightLight;
			label_VF_2GCells.Location = new Point(3, 50); // (87, 174);
			label_VF_2GCells.Name = "label_VF_2GCells";
			label_VF_2GCells.Size = new Size(37, 21);
			label_VF_2GCells.TabIndex = 99;
			label_VF_2GCells.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label_VF_3GCells
			// 
			label_VF_3GCells.BackColor = Color.DimGray;
			label_VF_3GCells.FlatStyle = FlatStyle.Flat;
			label_VF_3GCells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			label_VF_3GCells.ForeColor = SystemColors.ControlLightLight;
			label_VF_3GCells.Location = new Point(label_VF_2GCells.Right + 2, 50); // (126, 174);
			label_VF_3GCells.Name = "label_VF_3GCells";
			label_VF_3GCells.Size = new Size(37, 21);
			label_VF_3GCells.TabIndex = 100;
			label_VF_3GCells.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label_VF_4GCells
			// 
			label_VF_4GCells.BackColor = Color.DimGray;
			label_VF_4GCells.FlatStyle = FlatStyle.Flat;
			label_VF_4GCells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			label_VF_4GCells.ForeColor = SystemColors.ControlLightLight;
			label_VF_4GCells.Location = new Point(label_VF_3GCells.Right + 2, 50);
			label_VF_4GCells.Name = "label_VF_4GCells";
			label_VF_4GCells.Size = new Size(38, 21);
			label_VF_4GCells.TabIndex = 101;
			label_VF_4GCells.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label_TF_2GCells
			// 
			label_TF_2GCells.BackColor = Color.DimGray;
			label_TF_2GCells.FlatStyle = FlatStyle.Flat;
			label_TF_2GCells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			label_TF_2GCells.ForeColor = SystemColors.ControlLightLight;
			label_TF_2GCells.Location = new Point(label_VF_4GCells.Right + 5, 50); // (124, 50);
			label_TF_2GCells.Name = "label_TF_2GCells";
			label_TF_2GCells.Size = new Size(37, 21);
			label_TF_2GCells.TabIndex = 102;
			label_TF_2GCells.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label_TF_3GCells
			// 
			label_TF_3GCells.BackColor = Color.DimGray;
			label_TF_3GCells.FlatStyle = FlatStyle.Flat;
			label_TF_3GCells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			label_TF_3GCells.ForeColor = SystemColors.ControlLightLight;
			label_TF_3GCells.Location = new Point(label_TF_2GCells.Right + 2, 50);
			label_TF_3GCells.Name = "label_TF_3GCells";
			label_TF_3GCells.Size = new Size(38, 21);
			label_TF_3GCells.TabIndex = 103;
			label_TF_3GCells.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label_TF_4GCells
			// 
			label_TF_4GCells.BackColor = Color.DimGray;
			label_TF_4GCells.FlatStyle = FlatStyle.Flat;
			label_TF_4GCells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			label_TF_4GCells.ForeColor = SystemColors.ControlLightLight;
			label_TF_4GCells.Location = new Point(label_TF_3GCells.Right + 2, 50);
			label_TF_4GCells.Name = "label_TF_4GCells";
			label_TF_4GCells.Size = new Size(37, 21);
			label_TF_4GCells.TabIndex = 104;
			label_TF_4GCells.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label_Total_2GCells
			// 
			label_Total_2GCells.BackColor = Color.DimGray;
			label_Total_2GCells.FlatStyle = FlatStyle.Flat;
			label_Total_2GCells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			label_Total_2GCells.ForeColor = SystemColors.ControlLightLight;
			label_Total_2GCells.Location = new Point(label_TF_4GCells.Right + 5, 50);
			label_Total_2GCells.Name = "label_Total_2GCells";
			label_Total_2GCells.Size = new Size(38, 21);
			label_Total_2GCells.TabIndex = 105;
			label_Total_2GCells.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label_Total_3GCells
			// 
			label_Total_3GCells.BackColor = Color.DimGray;
			label_Total_3GCells.FlatStyle = FlatStyle.Flat;
			label_Total_3GCells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			label_Total_3GCells.ForeColor = SystemColors.ControlLightLight;
			label_Total_3GCells.Location = new Point(label_Total_2GCells.Right + 2, 50);
			label_Total_3GCells.Name = "label_Total_3GCells";
			label_Total_3GCells.Size = new Size(37, 21);
			label_Total_3GCells.TabIndex = 106;
			label_Total_3GCells.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label_Total_4GCells
			// 
			label_Total_4GCells.BackColor = Color.DimGray;
			label_Total_4GCells.FlatStyle = FlatStyle.Flat;
			label_Total_4GCells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			label_Total_4GCells.ForeColor = SystemColors.ControlLightLight;
			label_Total_4GCells.Location = new Point(label_Total_3GCells.Right + 2, 50);
			label_Total_4GCells.Name = "label_Total_4GCells";
			label_Total_4GCells.Size = new Size(37, 21);
			label_Total_4GCells.TabIndex = 107;
			label_Total_4GCells.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label_TotalCells
			// 
			label_TotalCells.BackColor = Color.DimGray;
			label_TotalCells.FlatStyle = FlatStyle.Flat;
			label_TotalCells.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			label_TotalCells.ForeColor = SystemColors.ControlLightLight;
			label_TotalCells.Location = new Point(label_Total_4GCells.Right + 5, 50);
			label_TotalCells.Name = "label_TotalCells";
			label_TotalCells.Size = new Size(34, 21);
			label_TotalCells.TabIndex = 108;
			label_TotalCells.TextAlign = ContentAlignment.MiddleCenter;
			
			Controls.Add(label_TotalCells);
			Controls.Add(label_Total_4GCells);
			Controls.Add(label_Total_3GCells);
			Controls.Add(label_Total_2GCells);
			Controls.Add(label_TF_4GCells);
			Controls.Add(label_TF_3GCells);
			Controls.Add(label_TF_2GCells);
			Controls.Add(label_VF_4GCells);
			Controls.Add(label_VF_3GCells);
			Controls.Add(label_VF_2GCells);
		}
	}
}
