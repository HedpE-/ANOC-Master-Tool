/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 23-09-2016
 * Time: 23:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using appCore.UI;
using appCore.Templates.Types;

namespace appCore.Netcool
{
	/// <summary>
	/// Description of Parser.
	/// </summary>
	public class Parser
	{
		string toParse;
		string parsedOutput = string.Empty;
		DataTable parsedTable = new DataTable();
		public List<Alarm> AlarmsList = new List<Alarm>();
		
		public Parser(string netcoolAlarms, bool generateOutput = true) {
			toParse = netcoolAlarms;
			ToDataTable();
			parseAlarms(generateOutput);
		}
		
		void ToDataTable() {
			while (toParse.Contains("\n-ProbableCause")) {
				toParse = toParse.Remove(toParse.IndexOf("\n-ProbableCause", StringComparison.Ordinal), 1);
			}
			
			string[] rows = toParse.Split('\n'); // Extrair todas as rows
			string[] columns = rows[0].Split('\t'); // Extrair o nome das colunas da primeira linha
			rows = rows.Where(x => !string.IsNullOrEmpty(x)).ToArray();
			string[] rowsFinal = new string[rows.GetLength(0) + 2];
			
			for (int a = 1; a < rows.GetLength(0); a++) // Remover a primeira string do array Rows visto ser a que tem o nome das columns
			{
				rowsFinal[a-1] = rows[a];
			}
			try
			{
				foreach (string column in columns)
				{
					parsedTable.Columns.Add(column, typeof(string)); // Criar as colunas na sheet
				}
				
				foreach (string row in rowsFinal)
				{
					if ( row != null) // Proteção para não rebentar pois o array rowsFinal tem sempre strings null (Não percebo porque)
					{
						string[] dividedRow = row.Split('\t'); // Dividir a row em colunas, uma por string no array
						DataRow DR = parsedTable.NewRow(); // Criar a Row com o formato da DataTable
						int a = 0;
						foreach (string column in dividedRow) // Adicionar o conteudo de cada column
						{
							if( a < 25 ) // Proteção para nao rebentar com o array
								DR[a] = column;
							a++;
						}
						parsedTable.Rows.Add(DR); // Adicionar a row a DataTable
					}
				}
			}
			catch ( Exception e3 )
			{
				FlexibleMessageBox.Show(e3.ToString(),"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		
		public void parseAlarms(bool generateOutput) {
			DataColumnCollection columns = parsedTable.Columns;
			
			foreach (DataRow row in parsedTable.Rows)
			{
				try {
					Alarm alarm = new Alarm(row, columns);
					AlarmsList.Add(alarm);
				}
				catch {	}
			}
			
			if(generateOutput)
				generateParsedOutput();
		}
		
		void generateParsedOutput() {
			parsedOutput = string.Empty;
			for(int c=0;c< AlarmsList.Count;c++) {
				parsedOutput += AlarmsList[c].ToString();
				if(c != AlarmsList.Count - 1)
					parsedOutput += Environment.NewLine + Environment.NewLine;
			}
		}
		
		public Outage GenerateOutage() {
			return new Outage(this);
		}
		
		public override string ToString()
		{
			return parsedOutput;
		}
	}
}
