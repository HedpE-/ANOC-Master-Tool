/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 18-08-2016
 * Time: 06:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using appCore.Settings;
using appCore.Shifts;
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

		public string FullName {
			get {
				return shiftsFile.FullName;
			}
			protected set { }
		}

		public bool Exists {
			get {
				if(shiftsFile != null)
					return shiftsFile.Exists;
				return false;
				
			}
			protected set { }
		}

		public Months LastMonthAvailable {
			get {
				int lastMonth = DateTime.Now.Month - 1;
				if(shiftsFile != null) {
					var nextMonthHeaderRange = package.Workbook.Worksheets[1].Cells[monthRanges[DateTime.Now.Month].ToString()];
					var nextMonthShifts = package.Workbook.Worksheets[1].Cells[4, nextMonthHeaderRange.Start.Column, 65, nextMonthHeaderRange.End.Column];
					foreach(var cell in nextMonthShifts) {
						if(cell.Value.ToString() == "M" || cell.Value.ToString() == "A" || cell.Value.ToString() == "N") {
							lastMonth++;
							break;
						}
					}
				}
				
				return (Months)lastMonth;
			}
			protected set { }
		}

		public int Year {
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
		
		public ShiftsFile(int year) {
			shiftsFile = UserFolder.getDBFile("shift*" + year + "*.xlsx");
			
			try {
				package = new ExcelPackage(shiftsFile);
			}
			catch {
				if(shiftsFile != null) {
					UserFolder.CreateTempFolder();
					FileInfo tempShiftsFile = shiftsFile.CopyTo(UserFolder.TempFolder + "\\" + shiftsFile.Name, true);
					package = new ExcelPackage(tempShiftsFile);
				}
				
			}
			
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
			if(package.File.FullName.Contains(UserFolder.TempFolder.FullName))
				UserFolder.ClearTempFolder();
		}
		
		public string GetShift(string name, DateTime date) {
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
		
		public string[] GetAllShiftsInMonth(string name, int month) {
			List<string> list = new List<string>();
			int personRow = FindPersonRow(name);
			var personRange = package.Workbook.Worksheets[1].Cells[monthRanges[month - 1].ToString().Replace("1",personRow.ToString())];
			for(int c = personRange.Start.Column;c <= personRange.End.Column;c++) {
				var cell = package.Workbook.Worksheets[1].Cells[personRow, c];
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
		
		int FindPersonRow(string name) {
			foreach(var cell in package.Workbook.Worksheets[1].Cells["c:c"]) {
				if(cell.Value != null) {
					string[] nameArr = name.ToUpper().Split(' ');
					if(cell.Text.ToUpper().RemoveDiacritics().Contains(nameArr[0].ToUpper().RemoveDiacritics()) &&
					   cell.Text.ToUpper().RemoveDiacritics().Contains(nameArr[1].ToUpper().RemoveDiacritics()))
						return cell.Start.Row;
				}
			}
			return 0;
		}
		
		public string getRole(string name) {
			int personRow = FindPersonRow(name);
			if(personRow > 3 && personRow < 12)
				return "Shift Leader";
			
			return "Agent";
		}
		
		public string GetClosureCode(string name) {
			int personRow = FindPersonRow(name);
			try { return package.Workbook.Worksheets[1].Cells[personRow, 3].Offset(0, -2).Text; }
			catch { return string.Empty; }
		}
		
		public string[] GetAllClosureCodes() {
			List<string> list = new List<string>();
			foreach(var cell in package.Workbook.Worksheets[1].Cells["a:a"]) {
				if(cell.Value != null) {
					if(cell.Value.ToString() != "Closure Code")
						list.Add(cell.Text);
				}
			}
			return list.ToArray();
		}
		
		string FindDayColumn(DateTime date) {
			foreach(var cell in package.Workbook.Worksheets[1].Cells[monthRanges[date.Month - 1].ToString().Replace("1","3")]) {
				if(cell.Value != null) {
					if(cell.Text == date.Day.ToString()) {
						return cell.Address.RemoveDigits();
					}
				}
			}
			return string.Empty;
		}
	}
}
