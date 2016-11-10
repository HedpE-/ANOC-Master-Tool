/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 30-07-2016
 * Time: 15:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Windows.Forms;
using appCore.Toolbox;

namespace appCore.Templates.Types
{
	/// <summary>
	/// Description of FailedCRQ.
	/// </summary>
//	[Serializable]
	public class FailedCRQ : Template
	{
		string inc = string.Empty;
		public string INC { get { return inc; } protected set { inc = value; } }
		string siteid = string.Empty;
		public string SiteId { get { return siteid; } protected set { siteid = value; } }
		string crq = string.Empty;
		public string CRQ { get { return crq; } protected set { crq = value; } }
		string febookedinname = string.Empty;
		public string FEBookedInName { get { return febookedinname; } protected set { febookedinname = value; } }
		string febookedintel = string.Empty;
		public string FEBookedInTel { get { return febookedintel; } protected set { febookedintel = value; } }
		bool fecalledanoc;
		public bool FECalledANOC { get { return fecalledanoc; } protected set { fecalledanoc = value; } }
		string crqcontacts = string.Empty;
		public string CrqContacts { get { return crqcontacts; } protected set { crqcontacts = value; } }
		string workperformed = string.Empty;
		public string WorkPerformed { get { return workperformed; } protected set { workperformed = value; } }
		string troubleshootingdone = string.Empty;
		public string TroubleshootingDone { get { return troubleshootingdone; } protected set { troubleshootingdone = value; } }
		string contractortofixfault_name = string.Empty;
		public string ContractorToFixFault_Name { get { return contractortofixfault_name; } protected set { contractortofixfault_name = value; } }
		string contractortofixfault_tel = string.Empty;
		public string ContractorToFixFault_Tel { get { return contractortofixfault_tel; } protected set { contractortofixfault_tel = value; } }
		DateTime contractortofixfault_date = new DateTime(1,1,1);
		public DateTime ContractorToFixFault_Date { get { return contractortofixfault_date; } protected set { contractortofixfault_date = value; } }
		string observations = string.Empty;
		public string Observations { get { return observations; } protected set { observations = value; } }
		string emailsubject = string.Empty;
		public string EmailSubject { get { return emailsubject; } protected set { emailsubject = value; } }
		string emailbody = string.Empty;
		public string EmailBody { get { return emailbody; } protected set { emailbody = value; } }
		
		public FailedCRQ() {
			LogType = "Failed CRQ";
		}
		
		public FailedCRQ(Control.ControlCollection controlsCollection)
		{
			try { INC = controlsCollection["INCTextBox"].Text; } catch (Exception) { }
			try { SiteId = controlsCollection["SiteIdTextBox"].Text; } catch (Exception) { }
			try { CRQ = controlsCollection["CRQTextBox"].Text; } catch (Exception) { }
			try { FEBookedInName = controlsCollection["FEBookedInGroupBox"].Controls["FEBookedIn_NameTextBox"].Text; } catch (Exception) { }
			try { FEBookedInTel = controlsCollection["FEBookedInGroupBox"].Controls["FEBookedIn_PhoneNumberTextBox"].Text; } catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["FEBookedIn_CalledANOCCheckBox"];
				FECalledANOC = cb.Checked;
			} catch (Exception) { }
			try { CrqContacts = controlsCollection["CRQContactsTextBox"].Text; } catch (Exception) { }
			try { WorkPerformed = string.IsNullOrEmpty(controlsCollection["WorkPerformedByFETextBox"].Text) ? "N/A" : controlsCollection["WorkPerformedByFETextBox"].Text; } catch (Exception) { }
			try { TroubleshootingDone = string.IsNullOrEmpty(controlsCollection["TroubleshootingDoneTextBox"].Text) ? "N/A" : controlsCollection["TroubleshootingDoneTextBox"].Text; } catch (Exception) { }
			try { ContractorToFixFault_Name = string.IsNullOrEmpty(controlsCollection["ContractorToFixFaultGroupBox"].Controls["ContractorToFixFault_NameTextBox"].Text) ? "None provided" : controlsCollection["ContractorToFixFaultGroupBox"].Controls["ContractorToFixFault_NameTextBox"].Text; } catch (Exception) { }
			try { ContractorToFixFault_Tel = controlsCollection["ContractorToFixFaultGroupBox"].Controls["ContractorToFixFault_PhoneNumberTextBox"].Text; } catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["ContractorToFixFaultGroupBox"].Controls["ContractorToFixFault_WillReturnCheckBox"];
				DateTimePicker dtp = (DateTimePicker)controlsCollection["ContractorToFixFaultGroupBox"].Controls["ContractorToFixFault_WillReturnDateTimePicker"];
				ContractorToFixFault_Date = cb.Checked ? dtp.Value : new DateTime(1,1,1); }
			catch (Exception) { }
			try { Observations = controlsCollection["ObservationsTextBox"].Text; } catch (Exception) { }
			
			try { EmailBody = buildHTMLemailBody(); } catch (Exception) { }
			try { EmailSubject = "Site " + SiteId + " - " + INC + " (Failed " + CRQ + ")"; } catch (Exception) { }
			fullLog = generateFullLog();
			LogType = "Failed CRQ";
		}
		
		public FailedCRQ(FailedCRQ template) {
			Tools.CopyProperties(this, template);
			fullLog = generateFullLog();
			LogType = "Failed CRQ";
		}
		
		public FailedCRQ(string[] log, DateTime date) {
			LoadFCRQ(log);
			try { EmailBody = buildHTMLemailBody(); } catch (Exception) { }
			try { EmailSubject = "Site " + SiteId + " - " + INC + " (Failed " + CRQ + ")"; } catch (Exception) { }
			string[] strTofind = { " - " };
			string[] time = log[0].Split(strTofind, StringSplitOptions.None)[0].Split(':');
			GenerationDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt16(time[0]), Convert.ToInt16(time[1]), Convert.ToInt16(time[2]));
			LogType = "Failed CRQ";
		}
		
		public void LoadFCRQ(string[] log)
		{
			string complete = string.Empty;
			string[] strTofind = { "\r\n" };
			int c = 1;
			INC = log[c++].Substring("INC raised: ".Length);
			if (log[c].Contains("Site: "))
				SiteId = log[c++].Substring("Site: ".Length);
			else
				SiteId = string.Empty;
			CRQ = log[c++].Substring("CRQ: ".Length);
			
			if(log[c].Contains("CRQ contacts:")) {
				complete = log[++c];
				for (c++; c < log.Length; c++) {
					if (!log[c].Contains("FE booked in:"))
						complete += Environment.NewLine + log[c];
					else break;
				}
				CrqContacts = complete;
			}
			else
				CrqContacts = log[c++].Substring("CRQ contact: ".Length);
			string[] temp = log[c++].Substring("FE booked in: ".Length).Split(',');
			FEBookedInName = temp[0];
			FEBookedInTel = temp[1].Substring(1);
			FECalledANOC = log[c++].Substring("Did FE call the ANOC after CRQ: ".Length) != "No";
			
			complete = string.Empty;
			if(log[c].Length > "Work performed by FE on site:".Length)
				complete = log[c++].Substring("Work performed by FE on site: ".Length);
			else {
				complete = log[++c];
				for (c++; c < log.Length; c++) {
					if (!log[c].Contains("Troubleshooting done with FE on site to recover affected cells:"))
						complete += Environment.NewLine + log[c];
					else break;
				}
				WorkPerformed = complete;
			}
			
			complete = string.Empty;
			if(log[c].Length > "Troubleshooting done with FE on site to recover affected cells:".Length)
				complete = log[c++].Substring("Troubleshooting done with FE on site to recover affected cells:".Length);
			else {
				complete = log[++c];
				for (c++; c < log.Length; c++) {
					if (!log[c].Contains("Contractor to fix the fault: "))
						complete += Environment.NewLine + log[c];
					else break;
				}
				TroubleshootingDone = complete;
			}
			
			if (log[c].Substring("Contractor to fix the fault: ".Length) == "None provided") {
				ContractorToFixFault_Name = "None provided";
				ContractorToFixFault_Tel = string.Empty;
				ContractorToFixFault_Date = new DateTime(1753, 1, 1);
				c++;
			}
			else {
				strTofind[0] = ", ";
				string[] temp2 = log[c].Substring("Contractor to fix the fault: ".Length).Split(strTofind, StringSplitOptions.None);
				ContractorToFixFault_Name = temp2[0];
				ContractorToFixFault_Tel = temp2[0] == "None provided" ? string.Empty : temp2[1];
				
				c++;
				if (log[c].Substring("Time to fix the fault: ".Length) == "None provided")
					ContractorToFixFault_Date = new DateTime(1753, 1, 1);
				else {
					// FIXME: Time or Date to fix the fault?
					try { ContractorToFixFault_Date = Convert.ToDateTime(log[c].Substring("Time to fix the fault: ".Length)); } catch {}
				}
			}
			c++;
			
			complete = string.Empty;
			if(log[c].Length > "Observations:".Length)
				complete = log[c++].Substring("Troubleshooting done with FE on site to recover affected cells: ".Length);
			else {
				if (c < log.Length) {
					complete += log[++c];
					for (c++; c < log.Length; c++)
						complete += Environment.NewLine + log[c];
				}
				Observations = complete;
			}
			fullLog = string.Join("\r\n", log.Where((val, idx) => idx != 0).ToArray());
		}
		
		string generateFullLog() {
			string template = "INC raised: " + INC + Environment.NewLine;
			
			template += "Site: " + SiteId + Environment.NewLine;
			template += "CRQ: " + CRQ + Environment.NewLine;
			template += "CRQ contacts:" + Environment.NewLine + CrqContacts + Environment.NewLine;
			
//			string[] lines = richTextBox16.Lines;
//			lines = lines.Length > 1 ? lines : new[] { richTextBox16.Text };
			
//			string[] lines = richTextBox16.Lines;
//			for (int i = 0; i < lines.Length; i++) {
//				if(!string.IsNullOrEmpty(lines[i]))
//					template += lines[i] + Environment.NewLine;
//			}
			template += "FE booked in: " + FEBookedInName + ", " + FEBookedInTel + Environment.NewLine;
			template += "Did FE call the ANOC after CRQ: ";
			template += (FECalledANOC ? "Yes" : "No") + Environment.NewLine;
			template += "Work performed by FE on site:";
			if (WorkPerformed != "N/A")
				template += Environment.NewLine + WorkPerformed + Environment.NewLine;
			else
				template += " N/A" + Environment.NewLine;
			
			template += "Troubleshooting done with FE on site to recover affected cells:";
			if (TroubleshootingDone != "N/A")
				template += Environment.NewLine + TroubleshootingDone + Environment.NewLine;
			else
				template += " N/A" + Environment.NewLine;
			
			template += "Contractor to fix the fault: ";
			if (ContractorToFixFault_Name != "None provided" || !string.IsNullOrEmpty(ContractorToFixFault_Tel))
				template += ContractorToFixFault_Name + ", " + ContractorToFixFault_Tel + Environment.NewLine;
			else
				template += "None provided" + Environment.NewLine;
			template += "Time to fix the fault: ";
			if (ContractorToFixFault_Date != new DateTime(1753, 1, 1))
				template += ContractorToFixFault_Date + Environment.NewLine;
			else
				template += "None provided" + Environment.NewLine;
			template += "Observations:";
			if (Observations != "N/A")
				template += Environment.NewLine + Observations;
			else
				template += " N/A";
			template += Environment.NewLine + Environment.NewLine + Signature;
			
			return template;
		}
		
		string buildHTMLemailBody() {
			const string HTMLstart = @"<html xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:w=""urn:schemas-microsoft-com:office:word"" xmlns:m=""http://schemas.microsoft.com/office/2004/12/omml"" xmlns=""http://www.w3.org/TR/REC-html40"">";
			const string HTMLhead = @"<head><meta http-equiv=Content-Type content=""text/html; charset=iso-8859-1""><meta name=Generator content=""Microsoft Word 14 (filtered medium)""><style><!--/* Font Definitions */@font-face{font-family:""Cambria Math"";panose-1:2 4 5 3 5 4 6 3 2 4;}@font-face{font-family:Calibri;panose-1:2 15 5 2 2 2 4 3 2 4;}@font-face{font-family:Tahoma;panose-1:2 11 6 4 3 5 4 4 2 4;}/* Style Definitions */p.MsoNormal, li.MsoNormal, div.MsoNormal{margin:0cm;margin-bottom:.0001pt;font-size:12.0pt;font-family:""Times New Roman"",""serif"";}a:link, span.MsoHyperlink{mso-style-priority:99;color:blue;text-decoration:underline;}a:visited, span.MsoHyperlinkFollowed{mso-style-priority:99;color:purple;text-decoration:underline;}p{mso-style-priority:99;mso-margin-top-alt:auto;margin-right:0cm;mso-margin-bottom-alt:auto;margin-left:0cm;font-size:12.0pt;font-family:""Times New Roman"",""serif"";}p.MsoAcetate, li.MsoAcetate, div.MsoAcetate{mso-style-priority:99;mso-style-link:""Balloon Text Char"";margin:0cm;margin-bottom:.0001pt;font-size:8.0pt;font-family:""Tahoma"",""sans-serif"";}span.TextodebaloCarcter{mso-style-name:""Texto de balão Carácter"";mso-style-priority:99;mso-style-link:""Texto de balão"";font-family:""Tahoma"",""sans-serif"";}span.EstiloCorreioElectrnico20{mso-style-type:personal-compose;font-family:""Calibri"",""sans-serif"";}p.BalloonText, li.BalloonText, div.BalloonText{mso-style-name:""Balloon Text"";mso-style-link:""Balloon Text Char"";margin:0cm;margin-bottom:.0001pt;font-size:12.0pt;font-family:""Times New Roman"",""serif"";}span.BalloonTextChar{mso-style-name:""Balloon Text Char"";mso-style-priority:99;mso-style-link:""Balloon Text"";font-family:""Tahoma"",""sans-serif"";mso-fareast-language:EN-GB;}.MsoChpDefault{mso-style-type:export-only;font-size:10.0pt;}@page WordSection1{size:612.0pt 792.0pt;margin:70.85pt 3.0cm 70.85pt 3.0cm;}div.WordSection1{page:WordSection1;}td.last {white-space: nowrap;}--></style><!--[if gte mso 9]><xml><o:shapedefaults v:ext=""edit"" spidmax=""1026"" /></xml><![endif]--><!--[if gte mso 9]><xml><o:shapelayout v:ext=""edit""><o:idmap v:ext=""edit"" data=""1"" /></o:shapelayout></xml><![endif]--></head>";
			const string HTMLbodyHeader = @"<body lang=EN-GB link=blue vlink=purple><div class=WordSection1><p style='margin:0cm;margin-bottom:.0001pt;line-height:110%'><span lang=EN-GB style='font-size:10.5pt;line-height:110%;font-family:""Calibri"",""sans-serif""'>Hello,<o:p></o:p></span></p><div><p class=MsoNormal style='line-height:110%'><span lang=EN-GB style='font-size:10.5pt;line-height:110%;font-family:""Calibri"",""sans-serif""'><o:p>&nbsp;</o:p></span></p>";
			const string HTMLtableRow = @"<tr><td width=""30%"" style='width:30%;border:solid windowtext 1.0pt;padding:0cm 5.4pt 0cm 5.4pt'><p style='margin:0cm;margin-bottom:.0001pt;line-height:110%'><b><span lang=EN-GB style='font-size:10.5pt;line-height:110%;font-family:""Calibri"",""sans-serif""'>{0}<o:p></o:p></span></b></p></td><td width=""70%"" style='width:70%;border:solid windowtext 1.0pt;border-left:none;padding:0cm 5.4pt 0cm 5.4pt'><p style='margin:0cm;margin-bottom:.0001pt;line-height:110%'><span lang=EN-GB style='font-size:10.5pt;line-height:110%;font-family:""Calibri"",""sans-serif""'>{1}<o:p></o:p></span></p></td></tr>";
			const string HTMLend = "</table></div></div></body></html>";
			
			// HTMLtableRow has {0} and {1} on each column to have String.Format input values
			
			string html = HTMLstart + HTMLhead + HTMLbodyHeader;
			html += @"<table class=MsoNormalTable border=0 cellspacing=0 cellpadding=0 width=""50%"" style='width:50.0%;border-collapse:collapse'>";
			html += String.Format(HTMLtableRow, "INC raised:", INC) + Environment.NewLine;
			html += String.Format(HTMLtableRow, "Site:", SiteId) + Environment.NewLine;
			html += String.Format(HTMLtableRow, "CRQ:", CRQ) + Environment.NewLine;
			html += String.Format(HTMLtableRow, "CRQ contacts:", CrqContacts) + Environment.NewLine;
			html += String.Format(HTMLtableRow, "FE booked in:", FEBookedInName + ", " + FEBookedInTel) + Environment.NewLine;
			html += String.Format(HTMLtableRow, "Did FE call the ANOC after CRQ:", FECalledANOC ? "Yes" : "No") + Environment.NewLine;
			html += String.Format(HTMLtableRow, "Work performed by FE on site:", WorkPerformed) + Environment.NewLine;
			html += String.Format(HTMLtableRow, "Troubleshooting done with FE on site to recover affected cells:", TroubleshootingDone) + Environment.NewLine;
			string contractorString = ContractorToFixFault_Name;
			if(contractorString != "None provided" && !string.IsNullOrEmpty(ContractorToFixFault_Tel))
				contractorString += ", " + ContractorToFixFault_Tel;
			html += String.Format(HTMLtableRow, "Contractor to fix the fault:", contractorString) + Environment.NewLine;
			html += String.Format(HTMLtableRow, "Time to fix the fault:", ContractorToFixFault_Date == new DateTime(1,1,1) ? "None provided" : String.Format("{0:dd/MM/yyyy H:mm}", ContractorToFixFault_Date)) + Environment.NewLine;
			html += String.Format(HTMLtableRow, "Observations:", Observations) + Environment.NewLine;
			
			return html + HTMLend;
		}
		
		public override string ToString()
		{
			if(string.IsNullOrEmpty(fullLog))
				generateFullLog();
			
			return fullLog;
		}
		
		public override FailedCRQ ToFailedCRQTemplate() {
			return new FailedCRQ(this);
		}
	}
}