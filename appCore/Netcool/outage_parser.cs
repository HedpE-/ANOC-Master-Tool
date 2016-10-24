/*
 * Created by SharpDevelop.
 * User: gonalvhf
 * Date: 22-11-2014
 * Time: 03:34
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq; // Preciso para usar o Distinct() para remover duplicados
using System.Text;
using appCore.Settings;


namespace appCore.Netcool
{
	/// <summary>
	/// Class for parsing the outage alarms from netcool
	/// </summary>
	public class outage_parser
	{
		string gsmEventTime;
		string umtsEventTime;
		string lteEventTime;
		List<string> POClist = new List<string>();
		public bool tableRemoved = false;
		
		public DataTable parse(string toparse)
		{
			DataTable parserTable = new DataTable(); // "Excel Sheet"
			
			while (toparse.Contains("\n-ProbableCause")) {
				toparse = toparse.Remove(toparse.IndexOf("\n-ProbableCause", StringComparison.Ordinal), 1);
			}
			
			string[] rows = toparse.Split('\n'); // Extrair todas as rows
			string[] columns = rows[0].Split('\t'); // Extrair o nome das colunas da primeira linha
			
			
			// ADICIONADO SUPORTE PARA VÁRIAS TABELAS DO NETCOOL ex. ALL ALARMS + BEACON
			
			int firstrow = 0; // Guarda a 1ª linha da tabela se o header não for igual
			for (int c = 0; c < rows.Length; c++) // Remover todos os headers com o nome das columns e linhas vazias
			{
				if (rows[c].Contains("Vendor") && rows[c].Contains("Summary") && rows[c].Contains("RNC/BSC") && rows[c].Contains("Location") && rows[c].Contains("Element")) {
					if (firstrow == 0) {
						if (columns.SequenceEqual(rows[c].Split('\t'))) {
							rows = rows.Where((source, index) => index != c).ToArray(); // Guardar todos o elementos da array menos o c
							c--;
						}
						else firstrow = c;
					}
					else {
						List<string> rowsList = rows.ToList();
						rowsList.RemoveRange(firstrow, c - firstrow);
						rows = rowsList.ToArray();
						c = firstrow - 1;
						firstrow = 0;
						tableRemoved = true;
					}
				}
				else {
					if (rows[c] == string.Empty) {
						rows = rows.Where((source, index) => index != c).ToArray(); // Guardar todos o elementos da array menos o c
						c--;
					}
				}
			}
			
			foreach (string column in columns)
			{
				parserTable.Columns.Add(column, typeof(string)); // Criar as colunas na sheet
			}
			
			int vendorIndex = parserTable.Columns["Vendor"].Ordinal;
			int elementIndex = parserTable.Columns["Element"].Ordinal;
			int nodeIndex = parserTable.Columns["Node"].Ordinal;
			int locationIndex = parserTable.Columns["Location"].Ordinal;
			foreach (string row in rows)
			{
				string[] dividedRow = row.Split('\t'); // Dividir a row em colunas, uma por string no array
				if(string.IsNullOrEmpty(dividedRow[elementIndex])) {
					switch(dividedRow[vendorIndex]) {
						case "HUAWEI":
							dividedRow[elementIndex] = dividedRow[nodeIndex];
							break;
						case "NSN":
							char[] nodeNSNcellID = dividedRow[nodeIndex].Substring(dividedRow[nodeIndex].Length - 3).ToCharArray();
							string elementID = null;
							if(nodeNSNcellID[2] == '4')
								elementID = "M";
							else
								elementID = "W";
							elementID += dividedRow[locationIndex].Substring(3);
							
							if(nodeNSNcellID[2] == '1' || nodeNSNcellID[2] == '4')
								elementID += "0";
							else
								elementID += nodeNSNcellID[2];
							elementID += nodeNSNcellID[0].ToString() + nodeNSNcellID[1].ToString();
							dividedRow[elementIndex] = elementID;
							break;
					}
				}
				DataRow DR = parserTable.NewRow(); // Criar a Row com o formato da DataTable
				int a = 0;
				foreach (string column in dividedRow) // Adicionar o conteudo de cada column
				{
					if( a < columns.GetLength(0) ) // Proteção para nao rebentar com o array
						DR[a] = column;
					a++;
				}
				parserTable.Rows.Add(DR); // Adicionar a row a DataTable
			}
			
			removeUnwanted(parserTable);
			
			return parserTable;
		}
		
		void removeUnwanted (DataTable parserTable) // Remove todas as row que não queremos contar
		{
			DataRow[] result = parserTable.Select("Element LIKE 'V%'");
			if(result.Length > 0)
				if(GlobalProperties.siteFinder_mainswitch)
					parserTable = parseLTE(parserTable,result);
				else
					parserTable = parseLTE(parserTable,result);
			
			int a = 0;
			while(a < parserTable.Rows.Count)
			{
				DataRow dr = parserTable.Rows[a];
				int vendorIndex = parserTable.Columns["Vendor"].Ordinal;
				int summaryIndex = parserTable.Columns["Summary"].Ordinal;
				int switchIndex = parserTable.Columns["RNC/BSC"].Ordinal; // Para filtrar 4G
				int locationIndex = parserTable.Columns["Location"].Ordinal; // Para guardar o site 4G
				int elementIndex = parserTable.Columns["Element"].Ordinal; // Para filtrar 4G quando iniciado em "V"
				int eventtimeIndex = parserTable.Columns["Last Occurrence"].Ordinal; // Encontrar a prosição da hora
				int countyIndex = parserTable.Columns["County"].Ordinal; // Encontrar a prosição da column Location
				
				// Filtro alarmes //
				
				switch (dr[vendorIndex].ToString())
				{
					case "ALU":
						if (!dr[summaryIndex].ToString().Contains("UNDERLYING_RESOURCE_UNAVAILABLE: State change to Disable"))
						{
							dr.Delete();
							if (a > 0)
								a--;
							continue;
						}
						break;
					case "ERICSSON":
						if (!((dr[summaryIndex].ToString().Contains("CELL LOGICAL CHANNEL AVAILABILITY SUPERVISION") && dr[summaryIndex].ToString().Contains("BCCH")) || dr[summaryIndex].ToString().Contains("UtranCell_ServiceUnavailable") || dr[summaryIndex].ToString().Contains("4G: Heartbeat Failure")))
						{
							dr.Delete();
							if (a > 0)
								a--;
							continue;
						}
						break;
					case "HUAWEI":
						if (!(dr[summaryIndex].ToString().Contains("Cell out of Service") || dr[summaryIndex].ToString().Contains("Cell Unavailable") || dr[summaryIndex].ToString().Contains("Local Cell Unusable") || dr[summaryIndex].ToString().Contains("eNodeB")))
						{
							dr.Delete();
							if (a > 0)
								a--;
							continue;
						}
						break;
					case "NSN":
						if (!(dr[summaryIndex].ToString().Contains("BCCH MISSING") || dr[summaryIndex].ToString().Contains("CELL FAULTY") || dr[summaryIndex].ToString().Contains("WCDMA CELL OUT OF USE") || dr[summaryIndex].ToString().Contains("P3 ENODEB: NE O&M")))
						{
							dr.Delete();
							if (a > 0)
								a--;
							continue;
						}
						break;
				}
				
				// Filtro cells
				// FIXME: Outage parser - 4G Cell Unnavailable alarms(Huawei) generates site count but 0 COOS
				
				char[] dr1 = dr[elementIndex].ToString().ToCharArray();
				if ( dr1.GetLength(0) > 1 )
				{
					if (dr1[0] == 'V') // Remover 4Gs
					{
						dr.Delete();
						a--;
					}
				}
				if ( a == parserTable.Rows.Count)
					break;
				a++;
			}
		}
		
		public static DataTable parseLTE(DataTable parserTable,DataRow[] lteRows)
		{
			List<string> lteSites = new List<string>();
			int cellIndex = parserTable.Columns["Element"].Ordinal; // Encontrar a posição da column Element para fazer o check VF ou TF
			int siteIndex = parserTable.Columns["Location"].Ordinal; // Encontrar a posição da column Location
			int switchIndex = parserTable.Columns["RNC/BSC"].Ordinal; // Para diferenciar mudar o switch dos sites 4G para filtrar mais fácilmente
			
			for(int c=0;c<lteRows.Length;c++)
				lteSites.Add(lteRows[c][siteIndex].ToString());
			
			lteSites = lteSites.Distinct().ToList();
			lteSites.Sort();
			
			if(lteSites.Count > 0) {
				for(int c = 0;c < lteSites.Count;c++)
					lteSites[c] = Convert.ToInt32(lteSites[c].Replace("RBS",string.Empty)).ToString();
				
				List<string> LTEcells = new List<string>();
				
				foreach(string site in lteSites) {
					DataView dv = MainForm.findCells(site);
					if(dv.Count > 0) {
						dv.RowFilter = "BEARER = '4G'";
						if(dv.Count > 0) {
							foreach (DataRowView cell in dv) {
								LTEcells.Add(site + " / " + cell[cell.Row.Table.Columns.IndexOf("CELL_NAME")].ToString());
							}
						}
					}
				}
				
				foreach(string site in lteSites) {
					List<string> allCells = LTEcells.Where(x => x.StartsWith(site, StringComparison.Ordinal)).ToList();
					
					string tmpSite = "RBS";
					for(int c = 0;c < 5 - site.Length;c++)
						tmpSite += "0";
					tmpSite += site;
					
					int siteRowIndex = 0;
					foreach (DataRow row in lteRows) {
						if(row[siteIndex].ToString() == tmpSite)
							break;
						siteRowIndex++;
					}
					
					foreach (string cell in allCells) {
						string[] strTofind = { " / " };
						string finalCell = cell.Split(strTofind, StringSplitOptions.None)[1];
						DataRow cellRow = parserTable.NewRow();
						cellRow.ItemArray = lteRows[siteRowIndex].ItemArray;
						cellRow[cellIndex] = finalCell;
						cellRow[switchIndex] = "4G";
						parserTable.Rows.Add(cellRow);
					}
				}
			}
			return parserTable;
		}
		
		public string genReport(DataTable parserTable,string Owner)
		{
			// Parse the output
			
			string[] sites = countSites(parserTable,Owner);
			
			if(sites.Length == 0)
				return string.Empty;
			
			string[] locations = countLocations(parserTable,Owner);
			string[] gsmCells = countCells2G(parserTable,Owner);
			string[] umtsCells = countCells3G(parserTable,Owner);
			string[] lteCells = countCells4G(parserTable,Owner);
			int nSites = sites.GetLength(0); // Contagem de sites
			int nLocations = locations.GetLength(0); // Contagem de locations
			int nCellsgsm = gsmCells.GetLength(0); // Contagem de cells 2G
			int nCellsumts = umtsCells.GetLength(0); // Contagem de cells 3G
			int nCellslte = lteCells.GetLength(0); // Contagem de cells 3G
			int cellTotal = nCellsgsm + nCellsumts + nCellslte; // Contagem total de cells
			
			foreach (string site in POClist) {
				string[] POCsite = site.Split('/');
				for (int c = 0;c < sites.Length - 1;c++) {
					if (sites[c] == POCsite[0]) {
						sites[c] += " - " + POCsite[1];
						break;
					}
				}
			}
			
			string final = cellTotal.ToString() + "x COOS (" + nSites.ToString();
			final += nSites == 1 ? " Site)" : " Sites)";
			final += Environment.NewLine + Environment.NewLine + "Locations (" + nLocations.ToString() + ")" + Environment.NewLine + ConvertStringArrayToString(locations) + Environment.NewLine + "Site List" + Environment.NewLine + ConvertStringArrayToString(sites);
			
			if ( nCellsgsm > 0 )
				final += Environment.NewLine + "2G Cells (" + nCellsgsm.ToString() + ") Event Time - " + Convert.ToDateTime(gsmEventTime).ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + ConvertStringArrayToString(gsmCells);
			if ( nCellsumts > 0)
				final += Environment.NewLine + "3G Cells (" + nCellsumts.ToString() + ") Event Time - " + Convert.ToDateTime(umtsEventTime).ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + ConvertStringArrayToString(umtsCells);
			if ( nCellslte > 0)
				final += Environment.NewLine + "4G Cells (" + nCellslte.ToString() + ") Event Time - " + Convert.ToDateTime(lteEventTime).ToString("dd/MM/yyyy HH:mm") + Environment.NewLine + ConvertStringArrayToString(lteCells);
			return final.Substring(0,final.Length - 2);
		}
		
		string[] countSites(DataTable parserTable,string Owner) // Contagem de sites, devolve lista alfabetica (.GetLength() para receber o numero de sites)
		{
			int siteIndex = parserTable.Columns["Location"].Ordinal; // Encontrar a posição da column Location
			int attrIndex = parserTable.Columns["Attributes"].Ordinal; // Encontrar a posição da column Attributes para armazenar POCs
			int cellIndex = parserTable.Columns["Element"].Ordinal; // Encontrar a posição da column Element para fazer o check VF ou TF
			List<string> sites = new List<string>(); // Criar array para armazenar todos os sites
			
			foreach ( DataRow loc in parserTable.Rows )
			{
				char[] dr1 = loc.ItemArray[cellIndex].ToString().ToCharArray();
				if(dr1[0] == 'T' || !char.IsDigit(dr1[dr1.Length - 1])) {
					if(Owner == "TF")
						sites.Add(loc.ItemArray[siteIndex].ToString());
				}
				else {
					if(Owner == "VF")
						sites.Add(loc.ItemArray[siteIndex].ToString());
				}
				// FIXME: Some POC sites don't get the POC label
				if (loc.ItemArray[attrIndex].ToString().Contains("POC")) {
					int onPos = loc.ItemArray[attrIndex].ToString().IndexOf("POC", StringComparison.Ordinal);
					string POC = string.Empty;
					foreach (char ch in loc.ItemArray[attrIndex].ToString().Substring(onPos, loc.ItemArray[attrIndex].ToString().Length - onPos)) {
						if (ch != '/' || ch != ' ') POC += ch;
						else break;
					}
					POClist.Add(loc.ItemArray[siteIndex] + "/" + POC);
				}
			}
			
			sites = sites.Distinct().ToList(); // Remover duplicados
			POClist = POClist.Distinct().ToList(); // Remover duplicados da lista de POCs
			sites.Sort(); // Ordenar
			sites = sites.Where(x => !string.IsNullOrEmpty(x)).ToList(); // Remover null/empty
			MainForm.sites = sites.ToArray();
			return sites.ToArray();
		}
		
		string[] countLocations (DataTable parserTable,string Owner) // Contar zonas
		{
			int locationIndex = parserTable.Columns["County"].Ordinal; // Encontrar a prosição da column Location
			int cellIndex = parserTable.Columns["Element"].Ordinal; // Encontrar a posição da column Element para fazer o check VF ou TF
			List<string> locations = new List<string>(); // Criar array para armazenar todas as locations
			
			foreach ( DataRow loc in parserTable.Rows )
			{
				char[] dr1 = loc.ItemArray[cellIndex].ToString().ToCharArray();
				if(dr1[0] == 'T' || !char.IsDigit(dr1[dr1.Length - 1])) {
					if(Owner == "TF") {
						if (loc.ItemArray[locationIndex].ToString() == string.Empty || loc.ItemArray[locationIndex].ToString() == "UNKNOWN")
							locations.Add(loc.ItemArray[locationIndex-1].ToString());
						else
							locations.Add(loc.ItemArray[locationIndex].ToString());
					}
				}
				else
					if(Owner == "VF") {
					if (loc.ItemArray[locationIndex].ToString() == string.Empty)
						locations.Add(loc.ItemArray[locationIndex-1].ToString());
					else
						locations.Add(loc.ItemArray[locationIndex].ToString());
				}
			}
			
			locations = locations.Distinct().ToList(); // Remover duplicados
			locations.Sort(); // Ordenar
			locations = locations.Where(x => !string.IsNullOrEmpty(x)).ToList(); // Remover null/empty
			return locations.ToArray();
		}
		
		private string[] countCells2G (DataTable parserTable,string Owner)
		{
			int elementIndex = parserTable.Columns["Element"].Ordinal; // Encontrar a posição da column Element para fazer o check VF ou TF
			int switchIndex = parserTable.Columns["RNC/BSC"].Ordinal; // Para diferenciar 2G/3G/4G
			int eventtimeIndex = parserTable.Columns["Last Occurrence"].Ordinal; // Encontrar a prosição da hora
			List<string> gsmCells = new List<string>(); // Criar array para armazenar todas as gsmCells
			List<string> gsmOutageTime = new List<string>(); // Criar array para armazenar todas as horas de caída das gsmCells
			
			foreach ( DataRow loc in parserTable.Rows )
			{
				char[] check = loc.ItemArray[switchIndex].ToString().ToCharArray();
				if ( check.GetLength(0) > 1 )
				{
					DataRow[] result = parserTable.Select("Element LIKE 'V%'");
					if ( check[0] == 'B' )
					{
						char[] dr1 = loc.ItemArray[elementIndex].ToString().ToCharArray();
						if(dr1[0] == 'T' || !char.IsDigit(dr1[dr1.Length - 1])) {
							if( Owner == "TF") {
								gsmCells.Add(loc.ItemArray[switchIndex].ToString() + " - " + loc.ItemArray[elementIndex].ToString());
								gsmOutageTime.Add(loc.ItemArray[eventtimeIndex].ToString());
							}
						}
						else {
							if( Owner == "VF") {
								gsmCells.Add(loc.ItemArray[switchIndex].ToString() + " - " + loc.ItemArray[elementIndex].ToString());
								gsmOutageTime.Add(loc.ItemArray[eventtimeIndex].ToString());
							}
						}
					}
				}
			}
			
			gsmCells = gsmCells.Distinct().ToList(); // Remover duplicados
			gsmOutageTime = gsmOutageTime.Distinct().ToList(); // Remover duplicados
			gsmCells.Sort(); // Ordenar
			gsmOutageTime.Sort(); // Ordenar
			gsmCells = gsmCells.Where(x => !string.IsNullOrEmpty(x)).ToList(); // Remover null/empty
			gsmOutageTime = gsmOutageTime.Where(x => !string.IsNullOrEmpty(x)).ToList(); // Remover null/empty
			if (gsmOutageTime.Count > 0) // Proteção para não rebentar no caso de a array estar empty (sem sites 2G)
				gsmEventTime = gsmOutageTime[0]; // Hora da 1ª caída
			return gsmCells.ToArray();
		}
		
		private string[] countCells3G (DataTable parserTable,string Owner)
		{
			int elementIndex = parserTable.Columns["Element"].Ordinal; // Encontrar a posição da column Element para fazer o check VF ou TF
			int eventtimeIndex = parserTable.Columns["Last Occurrence"].Ordinal; // Encontrar a prosição da hora
			int switchIndex = parserTable.Columns["RNC/BSC"].Ordinal; // Para diferenciar 2G/3G/4G
			List<string> umtsCells = new List<string>(); // Criar array para armazenar todas as umtsCells
			List<string> umtsOutageTime = new List<string>(); // Criar array para armazenar todas as horas de caída das umtsCells
			
			foreach ( DataRow loc in parserTable.Rows )
			{
				char[] check = loc.ItemArray[switchIndex].ToString().ToCharArray();
				if ( check.GetLength(0) > 1 )
				{
					if ( check[0] == 'R' )
					{
						char[] dr1 = loc.ItemArray[elementIndex].ToString().ToCharArray();
						if(dr1[0] == 'T' || !char.IsDigit(dr1[dr1.Length - 1])) {
							if( Owner == "TF") {
								umtsCells.Add(loc.ItemArray[switchIndex].ToString() + " - " + loc.ItemArray[elementIndex].ToString());
								umtsOutageTime.Add(loc.ItemArray[eventtimeIndex].ToString());
							}
						}
						else {
							if( Owner == "VF") {
								umtsCells.Add(loc.ItemArray[switchIndex].ToString() + " - " + loc.ItemArray[elementIndex].ToString());
								umtsOutageTime.Add(loc.ItemArray[eventtimeIndex].ToString());
							}
						}
					}
				}
			}
			umtsCells = umtsCells.Distinct().ToList(); // Remover duplicados
			umtsOutageTime = umtsOutageTime.Distinct().ToList(); // Remover duplicados
			umtsCells.Sort(); // Ordenar
			umtsOutageTime.Sort(); // Ordenar
			umtsCells = umtsCells.Where(x => !string.IsNullOrEmpty(x)).ToList(); // Remover null/empty
			umtsOutageTime = umtsOutageTime.Where(x => !string.IsNullOrEmpty(x)).ToList(); // Remover null/empty
			if (umtsOutageTime.Count > 0) // Proteção para não rebentar no caso de a array estar empty (sem sites 3G)
				umtsEventTime = umtsOutageTime[0]; // Hora da 1ª caída
			return umtsCells.ToArray();
		}
		
		string[] countCells4G (DataTable parserTable,string Owner)
		{
			int siteIndex = parserTable.Columns["Location"].Ordinal; // Encontrar a prosição da column Location
			int elementIndex = parserTable.Columns["Element"].Ordinal; // Encontrar a prosição da column Element
			int eventtimeIndex = parserTable.Columns["Last Occurrence"].Ordinal; // Encontrar a prosição da hora
			int switchIndex = parserTable.Columns["RNC/BSC"].Ordinal; // Para diferenciar 2G/3G/4G
			List<string> lteCells = new List<string>(); // Criar array para armazenar todas as umtsCells
			List<string> lteOutageTime = new List<string>(); // Criar array para armazenar todas as horas de caída das umtsCells
			
			int a = 0;
			foreach ( DataRow loc in parserTable.Rows )
			{
				char[] check = loc.ItemArray[switchIndex].ToString().ToCharArray();
				if (check.Length > 0) {
					if(check[0] == '4') {
						char[] dr1 = loc.ItemArray[elementIndex].ToString().ToCharArray();
						if(dr1[0] == 'T' || !char.IsDigit(dr1[dr1.Length - 1])) {
							if( Owner == "TF") {
								lteCells.Add(loc.ItemArray[elementIndex].ToString());
								lteOutageTime.Add(loc.ItemArray[eventtimeIndex].ToString());
							}
						}
						else {
							if( Owner == "VF") {
								lteCells.Add(loc.ItemArray[elementIndex].ToString());
								lteOutageTime.Add(loc.ItemArray[eventtimeIndex].ToString());
							}
						}
						a++;
					}
				}
			}
			
			lteCells = lteCells.Distinct().ToList(); // Remover duplicados
			lteOutageTime = lteOutageTime.Distinct().ToList(); // Remover duplicados
			lteCells.Sort(); // Ordenar
			lteOutageTime.Sort(); // Ordenar
			lteOutageTime = lteOutageTime.Where(x => !string.IsNullOrEmpty(x)).ToList(); // Remover null/empty
			if (lteOutageTime.Count > 0) // Proteção para não rebentar no caso de a array estar empty (sem sites 3G)
				lteEventTime = lteOutageTime[0]; // Hora da 1ª caída
			return lteCells.ToArray();
		}
		
		private string ConvertStringArrayToString(string[] array)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string value in array)
			{
				builder.Append(value);
				builder.Append(Environment.NewLine);
			}
			return builder.ToString();
		}
		
		public string bulkCi (string[] sites)
		{
			string[] strTofind = { " - " };
			for (int c = 0;c < sites.Length - 1;c++) {
				if (sites[c].Split(strTofind, StringSplitOptions.None).Length > 1) {
					sites[c] = sites[c].Split(strTofind, StringSplitOptions.None)[0];
				}
			}
			
			for (int a = 0;a < sites.Length;a++)
			{
				string final = sites[a].Replace("RBS",string.Empty);
				switch (final.Length)
				{
						case 1 : final = "000" + final;break;
						case 2 : final = "00" + final;break;
						case 3 : final = "0" + final;break;
						default :
						{
							if (final.Length > 4) {
								if (final.Substring(0,1) == "0") {
									final = final.Substring(1,final.Length - 1);
								}
							}
							break;
						}
				}
				sites[a] = final;
			}
			sites = sites.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // Remover null/empty
			return ConvertBulkCi(sites);
		}
		
		string ConvertBulkCi(string[] array)
		{
			StringBuilder builder = new StringBuilder();
			int bulkcount = 1;
			foreach (string value in array)
			{
				if ((bulkcount % 50) == 0) builder.Append(Environment.NewLine + Environment.NewLine);
				builder.Append(value);
				builder.Append(';');
				bulkcount++;
			}
			return builder.ToString();
		}
	}
}