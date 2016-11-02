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
using System.Collections.Generic;
using Excel;
using appCore.Settings;
using appCore.Toolbox;

namespace appCore.DB
{
    /// <summary>
    /// Description of ShiftsFile.
    /// </summary>
    public class ShiftsFile
    {
        FileInfo shiftsFile;

        public List<DataTable> monthTables = new List<DataTable>();

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
                if (shiftsFile != null)
                    return shiftsFile.Exists;
                else
                    return false;
            }
            protected set { }
        }

        public Tools.Months LastMonthAvailable
        {
            get
            {
                return (Tools.Months)(monthTables.Count - 1);
            }
            protected set { }
        }

        public int Year
        {
            get;
            private set;
        }

        public ShiftsFile(int year = 2016)
        {
            shiftsFile = UserFolder.getDBFile("shift*.xlsx");
            if (shiftsFile != null)
                monthTables = importShiftsTable();
            Year = year;
        }

        List<DataTable> importShiftsTable()
        {
            FileStream stream = shiftsFile.Open(FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet ds = excelReader.AsDataSet();
            DataTable dtTable = ds.Tables[0];
            excelReader.Close();

            // divide datatable code http://stackoverflow.com/questions/22312385/splitting-a-datatable-into-2-using-a-column-index
            int days = (int)(DateTime.Now - new DateTime(2016, 1, 1)).TotalDays;
            byte lastMonth = (byte)(DateTime.Now.Month - 1); // - 1 to get enum starting on 0
            List<DataTable> dtTablesList = new List<DataTable>();

            // Build TableA with columns F1, F2 and F3 that will go to all tables plus range from start of month until start of next month
            // Added 4th Column named AbsName containing the UpperCase + Diacritics removed from persons name
            DataTable TableA = new DataTable();
            DataColumn[] aCols = dtTable.Columns.Cast<DataColumn>()
                .Where(c => c.Ordinal < 3)
                .Select(c => new DataColumn(c.ColumnName, c.DataType))
                .ToArray();
            TableA.Columns.AddRange(aCols);
            TableA.Columns.Add(new DataColumn("AbsName", typeof(string)));
            foreach (DataRow row in dtTable.Rows)
            {
                DataRow aRow = TableA.Rows.Add();
                foreach (DataColumn aCol in TableA.Columns)
                {
                    if (aCol.ColumnName != "AbsName")
                        aRow.SetField(aCol, row[aCol.ColumnName]);
                    else
                    {
                        if (!string.IsNullOrEmpty(row["Column3"].ToString()) &&
                           row["Column3"].ToString() != "Name" &&
                           row["Column3"].ToString() != "Morning" &&
                           !row["Column3"].ToString().Contains("Intermediate") &&
                           row["Column3"].ToString() != "Afternoon" &&
                           row["Column3"].ToString() != "Night" &&
                           row["Column3"].ToString() != "TEF Customer")

                            aRow.SetField(aCol, Tools.RemoveDiacritics(row["Column3"].ToString()).ToUpper());
                    }
                }
            }

            // Check if next month is available
            if (lastMonth != 11)
            {
                int firstColumn = dtTable.Columns.Cast<DataColumn>()
                    .Where(c => dtTable.Rows[0][c].ToString().Contains(Enum.GetName(typeof(Tools.Months), lastMonth + 1)))
                    .Select(c => c.Ordinal)
                    .ToArray()[0];
                int lastColumn = 0;
                for (int i = firstColumn; i < dtTable.Columns.Count; i++)
                {
                    if (dtTable.Rows[0][i].ToString() == Enum.GetName(typeof(Tools.Months), lastMonth + 2))
                    { // lastMonth + 2 to get end of next month's table
                        lastColumn = i - 1;
                        break;
                    }
                }
                int nullCount = 0;

                for (int i = firstColumn; i <= lastColumn; i++)
                {
                    if (string.IsNullOrEmpty(dtTable.Rows[29][i].ToString())) // check values on Row 29, anyone's shifts
                        nullCount++;
                }
                if (nullCount < (lastColumn - firstColumn) - 10)
                    lastMonth++;
            }

            for (int i = 0; i <= lastMonth; i++)
            {
                string curMonth = Enum.GetName(typeof(Tools.Months), i);
                int monthFirstColumn = dtTable.Columns.Cast<DataColumn>()
                    .Where(c => dtTable.Rows[0][c].Equals(curMonth))
                    .Select(c => c.Ordinal)
                    .ToArray()[0];
                int monthLastColumn = 0;
                for (int c = monthFirstColumn; c < dtTable.Columns.Count; c++)
                {
                    if (dtTable.Columns[c].Ordinal > monthFirstColumn)
                    {
                        if (!string.IsNullOrEmpty(dtTable.Rows[0][dtTable.Columns[c].Ordinal].ToString()))
                        {
                            monthLastColumn = dtTable.Columns[c].Ordinal;
                            break;
                        }
                    }
                }
                DataColumn[] bCols = dtTable.Columns.Cast<DataColumn>()
                    .Where(c => c.Ordinal >= monthFirstColumn && c.Ordinal <= monthLastColumn)
                    .Select(c => new DataColumn(c.ColumnName, c.DataType))
                    .ToArray();

                DataTable TableB = TableA.Copy();
                TableB.Columns.AddRange(bCols);
                for (int c = 0; c < dtTable.Rows.Count; c++)
                {
                    DataRow bRow = TableB.Rows[c];
                    foreach (DataColumn bCol in bCols)
                    {
                        bRow.SetField(bCol, dtTable.Rows[c][bCol.ColumnName]);
                    }
                }

                for (int c = 0; c < TableB.Columns.Count; c++)
                {
                    if (!string.IsNullOrEmpty(TableB.Rows[2][TableB.Columns[c].ColumnName].ToString()))
                        TableB.Columns[c].ColumnName = "Day" + TableB.Rows[2][TableB.Columns[c].ColumnName];
                }
                TableB.TableName = curMonth;
                dtTablesList.Add(TableB);
            }
            return dtTablesList;
        }
    }
}
