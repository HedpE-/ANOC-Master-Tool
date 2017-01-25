/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 18-08-2016
 * Time: 06:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using appCore.Settings;
using appCore.Shifts;
using appCore.Toolbox;
using OfficeOpenXml;

namespace appCore.DB
{
	/// <summary>
	/// Description of ShiftsFile.
	/// </summary>
	public class ShiftsFile
	{
		FileInfo shiftsFile;
		ExcelPackage package;
		
		ArrayList monthRanges;

		public string FullName
		{
			get
			{
				return shiftsFile.FullName;
			}
			protected set { }
		}

		public bool Exists
		{
			get
			{
				if(shiftsFile != null)
					return shiftsFile.Exists;
				return false;
				
			}
			protected set { }
		}

//		public Tools.Months LastMonthAvailable
//		{
//			get
//			{
//				return (Tools.Months)(monthTables.Count - 1);
//			}
//			protected set { }
//		}

		public int Year
		{
			get;
			private set;
		}
		
		public List<string> ShiftLeaders {
			get;
			private set;
		}
		
		public List<string> Agents {
			get;
			private set;
		}

		public ShiftsFile(int year)
		{
			shiftsFile = UserFolder.getDBFile("shift*" + year + "*.xlsx");
//			shiftsFile = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Shift 2017_JAN - Copy.xlsx");
			
			try { package = new ExcelPackage(shiftsFile); } finally { }
			
			if (package.File != null) {
				var allMergedCells = package.Workbook.Worksheets[1].MergedCells;
				monthRanges = new ArrayList();
				foreach(string address in allMergedCells.List) {
					string[] temp = address.Split(':');
					if(temp[0].RemoveLetters() != "1")
						continue;
					if(temp[1].RemoveLetters() != "1")
						continue;
					monthRanges.Add(address);
				}
				
				SortAlphabetLength alphaLen = new SortAlphabetLength();
				monthRanges.Sort(alphaLen);
				
				bool slListEnd = false;
				foreach(var cell in package.Workbook.Worksheets[1].Cells["a:a"]) {
					if(cell.Value != null) {
						if(cell.Text != "Closure Code") {
							if(!slListEnd) {
								if(ShiftLeaders == null)
									ShiftLeaders = new List<string>();
								ShiftLeaders.Add(cell.Offset(0, 2).Text);
							}
							else {
								if(Agents == null)
									Agents = new List<string>();
								Agents.Add(cell.Offset(0, 2).Text);
							}
						}
					}
					else {
						if(!slListEnd && cell.Offset(0, 2).Value != null)
							slListEnd = cell.Offset(0, 2).Text == "Morning";
					}
				}
			}
			Year = year;
		}
		
		public String GetShift(String name, DateTime date) {
			int personRow = FindPersonRow(name);
			foreach(var cell in package.Workbook.Worksheets[1].Cells[monthRanges[date.Month - 1].ToString().Replace('1','3')]) {
				if(cell.Value != null) {
					if(cell.Text == date.Day.ToString())
						return package.Workbook.Worksheets[1].Cells[personRow, cell.Start.Column].Text;
				}
//				else
//					return string.Empty;
			}
			return string.Empty;
		}
		
		public String[] GetAllShiftsInMonth(String name, int month) {
			List<string> list = new List<string>();
			int personRow = FindPersonRow(name);
			foreach(var cell in package.Workbook.Worksheets[1].Cells[monthRanges[month - 1].ToString().Replace("1",personRow.ToString())]) {
				if(cell.Value == null)
					list.Add(string.Empty);
				else
					list.Add(cell.Text);
			}
			return list.ToArray();
		}
		
		public List<SingleShift> GetWholeShift(string shift, DateTime date) {
			if(shift.StartsWith("H") || shift == "B")
				return null;
			
			string dayColumn = FindDayColumn(date);
			var columnRange = package.Workbook.Worksheets[1].Cells[dayColumn.ToLower() + ":" + dayColumn.ToLower()];
			List<SingleShift> foundRows = new List<SingleShift>();
			switch (shift) {
				case "M": case "MT": case "QM":
					foreach(var cell in columnRange) {
						if(cell.Start.Row > 3) {
							if(cell.Value != null) {
								if(cell.Text != "A" && cell.Text != "N" && !cell.Text.StartsWith("H") && cell.Text != "HA" && cell.Text != "L" && cell.Text != "B" && !cell.Text.IsAllDigits())
									foundRows.Add(new SingleShift(package.Workbook.Worksheets[1].Cells[cell.Start.Row, 3].Text, cell.Text, date));
							}
						}
					}
					break;
				case "A":
					foreach(var cell in columnRange) {
						if(cell.Start.Row > 3) {
							if(cell.Value != null) {
								if(cell.Text != "M" && cell.Text != "MT" && cell.Text != "QM" && cell.Text != "N" && !cell.Text.StartsWith("H") && cell.Text != "HA" && cell.Text != "L" && cell.Text != "B" && !cell.Text.IsAllDigits())
									foundRows.Add(new SingleShift(package.Workbook.Worksheets[1].Cells[cell.Start.Row, 3].Text, cell.Text, date));
							}
						}
					}
					break;
				case "N":
					foreach(var cell in columnRange) {
						if(cell.Start.Row > 3) {
							if(cell.Value != null) {
								if(cell.Text == "N")
									foundRows.Add(new SingleShift(package.Workbook.Worksheets[1].Cells[cell.Start.Row, 3].Text, cell.Text, date));
							}
						}
					}
					break;
				default:
					foreach(var cell in columnRange) {
						if(cell.Start.Row > 3) {
							if(cell.Value != null) {
								if(cell.Text != "N" && !cell.Text.StartsWith("H") && cell.Text != "HA" && cell.Text != "L" && cell.Text != "B" && !cell.Text.IsAllDigits())
									foundRows.Add(new SingleShift(package.Workbook.Worksheets[1].Cells[cell.Start.Row, 3].Text, cell.Text, date));
							}
						}
					}
					
					break;
			}
			
			return foundRows;
		}
		
		public String GetClosureCode(String name) {
			foreach(var cell in package.Workbook.Worksheets[1].Cells["c:c"]) {
				if(cell.Value != null) {
					string[] nameArr = name.Split(' ');
					if(cell.Text.Contains(nameArr[0]) && cell.Text.Contains(nameArr[1]))
						return cell.Offset(0, -2).Text;
				}
			}
			return string.Empty;
		}
		
		public String[] GetAllClosureCodes() {
			List<string> list = new List<string>();
			foreach(var cell in package.Workbook.Worksheets[1].Cells["a:a"]) {
				if(cell.Value != null) {
					if(cell.Value.ToString() != "Closure Code")
						list.Add(cell.Text);
				}
			}
			return list.ToArray();
		}
		
		int FindPersonRow(String name) {
			foreach(var cell in package.Workbook.Worksheets[1].Cells["c:c"]) {
				if(cell.Value != null) {
					string[] nameArr = name.Split(' ');
					if(cell.Text.Contains(nameArr[0]) && cell.Text.Contains(nameArr[1]))
						return cell.Start.Row;
				}
			}
			return 0;
		}
		
		String FindDayColumn(DateTime date) {
			foreach(var cell in package.Workbook.Worksheets[1].Cells[monthRanges[date.Month - 1].ToString().Replace("1","3")]) {
				if(cell.Value != null) {
					if(cell.Text == date.Day.ToString()) {
						return cell.Address.RemoveDigits();
					}
				}
			}
			return string.Empty;
		}
		
//		List<DataTable> importShiftsTable(int month)
//		{
//			FileStream stream = null;
//			try {
//				stream = shiftsFile.Open(FileMode.Open, FileAccess.Read);
//			}
//			catch (Exception) {
//				UserFolder.CreateTempFolder();
//				FileInfo tempShiftsFile = shiftsFile.CopyTo(UserFolder.TempFolder.FullName + "\\" + shiftsFile.Name, true);
//				stream = tempShiftsFile.Open(FileMode.Open, FileAccess.Read);
//			}
//			return null;
//		}
	}
}
