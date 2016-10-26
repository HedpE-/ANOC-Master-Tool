/*
 * Created by SharpDevelop.
 * User: gonalvhf
 * Date: 23-11-2014
 * Time: 03:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Windows.Forms;
using System.Linq; // Preciso para usar o Distinct() para remover duplicados
using System.Text;

namespace appCore.Netcool
{
	/// <summary>
	/// Class to parse alarms from netcool for the remedy template
	/// </summary>
	public class netcool_parser
	{		
		DataTable parserTable = new DataTable(); // "Excel Sheet"
		
		public string parse(string toparse)
		{
			while (toparse.Contains("\n-ProbableCause")) {
				toparse = toparse.Remove(toparse.IndexOf("\n-ProbableCause", StringComparison.Ordinal), 1);
			}
			
			string[] rows = toparse.Split('\n'); // Extrair todas as rows
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
					parserTable.Columns.Add(column, typeof(string)); // Criar as colunas na sheet
				}
				
				foreach (string row in rowsFinal)
				{
					if ( row != null) // Proteção para não rebentar pois o array rowsFinal tem sempre strings null (Não percebo porque)
					{
						string[] dividedRow = row.Split('\t'); // Dividir a row em colunas, uma por string no array
						DataRow DR = parserTable.NewRow(); // Criar a Row com o formato da DataTable
						int a = 0;
						foreach (string column in dividedRow) // Adicionar o conteudo de cada column
						{
							if( a < 25 ) // Proteção para nao rebentar com o array
								DR[a] = column;
							a++;
						}
						parserTable.Rows.Add(DR); // Adicionar a row a DataTable
					}
				}
			}
			catch ( Exception e3 )
			{
				MessageBox.Show(e3.ToString(),"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return parseOutput();
		}
		
		string parseOutput ()
		{
			string[] final = new string[parserTable.Rows.Count];
			int ab = 0;
			int lastoccurIndex = 0;
			DataColumnCollection columns = parserTable.Columns;

			lastoccurIndex = columns.Contains("Last Occurrence") ? parserTable.Columns["Last Occurrence"].Ordinal : parserTable.Columns["LastOccurrence"].Ordinal; // Encontrar a posição da column Last Occurence
			int switchIndex = parserTable.Columns["RNC/BSC"].Ordinal; // Encontrar a prosição da column RNC/BSC
			int locationIndex = parserTable.Columns["Location"].Ordinal; // Encontrar a prosição da column Location
			int elementIndex = parserTable.Columns["Element"].Ordinal; // Encontrar a prosição da column Element
			int summaryIndex = parserTable.Columns["Summary"].Ordinal; // Encontrar a prosição da column Summary
			int countIndex = parserTable.Columns["Count"].Ordinal; // Encontrar a prosição da column Count
			
			foreach (DataRow row in parserTable.Rows)
			{
				string summary = row.ItemArray[summaryIndex].ToString();

				final[ab] = row.ItemArray[lastoccurIndex].ToString() + " - " + parseSummary(summary) + Environment.NewLine + row.ItemArray[switchIndex].ToString() + " > " + row.ItemArray[locationIndex].ToString() + " > " + row.ItemArray[elementIndex].ToString() + System.Environment.NewLine + "Alarm count: " + row.ItemArray[countIndex].ToString();
				ab++;
			}

			return ConvertStringArrayToString(final);
		}
		
		private string parseSummary(string toParse)
		{

			toParse = toParse.TrimStart(' ');
			toParse = toParse.TrimEnd(' ');
			
			if ( toParse.Contains("CELL LOGICAL") )
			{
				toParse = toParse.Replace("P1", string.Empty);
				toParse = toParse.Replace("P3", string.Empty);
				toParse = toParse.Replace("RBS", string.Empty);
				toParse = toParse.Replace("NODEB", string.Empty);
				toParse = toParse.Replace("ENODEB", string.Empty);
				toParse = toParse.Replace("SITE", string.Empty);
				toParse = toParse.Replace(":", string.Empty);
				int Pos = toParse.IndexOf("(", StringComparison.Ordinal);
				
				if (Pos != -1)
				{
					string channelType = string.Empty;
					int a = 0;
					foreach (char ch in toParse)
					{
						if(ch == '(')
							a++;
						if(ch != '(')
							if (a == 0)
								continue;
						channelType += ch.ToString();
						if(ch == ')')
							break;
					}
					if ( channelType == "(TCH FR 1)" )
					{
						int onPos = toParse.IndexOf("CELL = ", StringComparison.Ordinal);
						if (onPos != -1)
						{
							string cell = string.Empty;
							foreach (char ch in toParse.Substring(onPos + 6, toParse.IndexOf(" (", StringComparison.Ordinal) - (onPos + 6)))
							{
								if ( ch != ' ' ) cell += ch;
							}
							channelType += Environment.NewLine + "TCH Cell Degraded (" + cell + ")";
						}
					}
					if ( toParse.Contains("on") )
					{
						int onPos = toParse.IndexOf("on", StringComparison.Ordinal);
						
						if (onPos != -1)
						{
							toParse = toParse.Remove(onPos, toParse.Length - onPos);
						}
					}
					toParse = toParse + channelType;
					toParse = toParse.TrimStart(' ');
					toParse = toParse.TrimEnd(' ');
					return toParse;
				}
			}
			
			if ( toParse.Contains("RADIO X-CEIVER ADMINISTRATION MANAGED OBJECT FAULT") )
			{
				toParse = toParse.Replace("P1", string.Empty);
				toParse = toParse.Replace("P3", string.Empty);
				toParse = toParse.Replace("RBS", string.Empty);
				toParse = toParse.Replace("NODEB", string.Empty);
				toParse = toParse.Replace("ENODEB", string.Empty);
				toParse = toParse.Replace("SITE", string.Empty);
				toParse = toParse.Replace(":", string.Empty);
				int Pos = toParse.IndexOf("MO = ", StringComparison.Ordinal);
				
				if (Pos != -1)
				{
					string moType = string.Empty;
					int a = 0;
					foreach (char ch in toParse)
					{
						if(ch == '=')
							a++;
						if(ch != '=')
							if (a == 0)
								continue;
						moType += ch.ToString();
						if(ch == ':')
							break;
					}
					if ( toParse.Contains("on") )
					{
						int onPos = toParse.IndexOf("on", StringComparison.Ordinal);
						
						if (onPos != -1)
						{
							toParse = toParse.Remove(onPos, toParse.Length - onPos);
						}
					}
					moType = moType.Replace("=", "-");
					moType = moType.Remove(0,2);
					toParse = toParse.Replace("BTS", string.Empty);
					toParse = toParse + Environment.NewLine + moType;
					toParse = toParse.TrimStart(' ');
					toParse = toParse.TrimEnd(' ');
					return toParse;
				}
			}
			
			return toParse;
		}
		
		string ConvertStringArrayToString(string[] array)
		{
			StringBuilder builder = new StringBuilder();
			int strcount = 1;
			foreach (string value in array)
			{
				builder.Append(value);
				if (strcount != array.Length) {
					builder.Append(Environment.NewLine + Environment.NewLine);
					strcount++;
				}
			}
			return builder.ToString();
		}
	}
}
