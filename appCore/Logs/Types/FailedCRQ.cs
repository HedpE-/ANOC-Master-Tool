/*
 * Created by SharpDevelop.
 * User: Caramelos
 * Date: 30-07-2016
 * Time: 15:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using appCore.Logs;
using System.Reflection;

namespace appCore.Logs.Types
{
	/// <summary>
	/// Description of FailedCRQ.
	/// </summary>
	public class FailedCRQ : Log
	{
		public string INC { get; set; }
		public string SiteId { get; set; }
		public string CRQ { get; set; }
		public string FEBookedIn_Name { get; set; }
		public string FEBookedIn_Tel { get; set; }
		public bool FECalledANOC { get; set; }
		public string Contacts { get; set; }
		public string WorkPerformed { get; set; }
		public string TroubleshootingDone { get; set; }
		public string ContractorToFixFault_Name { get; set; }
		public string ContractorToFixFault_Tel { get; set; }
		public DateTime ContractorToFixFault_Date { get; set; }
		public string Observations { get; set; }
		public string Signature { get; set; }
		
		protected FailedCRQ() { }
		
		public FailedCRQ(Control.ControlCollection controlsCollection)
		{
			try { INC = controlsCollection["textBox16"].Text; } catch (Exception) { }
			try { SiteId = controlsCollection["textBox27"].Text; } catch (Exception) { }
			try { CRQ = controlsCollection["textBox17"].Text; } catch (Exception) { }
			try { FEBookedIn_Name = controlsCollection["textBox13"].Text; } catch (Exception) { }
			try { FEBookedIn_Tel = string.IsNullOrEmpty(controlsCollection["textBox12"].Text) ? "None" : controlsCollection["textBox5"].Text; } catch (Exception) { }
			try {
				CheckBox cb = (CheckBox)controlsCollection["checkBox8"];
				FECalledANOC = cb.Checked;
			} catch (Exception) { }
			try { Contacts = controlsCollection["richTextBox4"].Text; } catch (Exception) { }
			try { WorkPerformed = string.IsNullOrEmpty(controlsCollection["richTextBox1"].Text) ? "N/A" : controlsCollection["richTextBox1"].Text; } catch (Exception) { }
			try { TroubleshootingDone = string.IsNullOrEmpty(controlsCollection["richTextBox2"].Text) ? "N/A" : controlsCollection["richTextBox2"].Text; } catch (Exception) { }
			try { ContractorToFixFault_Name = string.IsNullOrEmpty(controlsCollection["textBox11"].Text) ? "None provided" : controlsCollection["textBox11"].Text; } catch (Exception) { }
			try { ContractorToFixFault_Tel = controlsCollection["textBox10"].Text; } catch (Exception) { }
			try { 
				CheckBox cb = (CheckBox)controlsCollection["checkBox7"];
				DateTimePicker dtp = (DateTimePicker)controlsCollection["dateTimePicker2"];
				ContractorToFixFault_Date = cb.Checked ? dtp.Value : new DateTime(0,0,0); }
			catch (Exception) { }
			try { Observations = controlsCollection["richTextBox3"].Text; } catch (Exception) { }
			
		}
		
		public FailedCRQ(appCore.Templates.Types.FailedCRQ template) {
			Log log = new FailedCRQ();
			CopyProperties(log, template);
			generateFullLog(template.EmailBody);
		}
		
		public static void CopyProperties(object dst, object src)
		{
			PropertyInfo[] srcProperties = src.GetType().GetProperties();
			dynamic dstType = dst.GetType();

			if (srcProperties == null | dstType.GetProperties() == null) {
				return;
			}

			foreach (PropertyInfo srcProperty in srcProperties) {
				PropertyInfo dstProperty = dstType.GetProperty(srcProperty.Name);

				if (dstProperty != null) {
					if(dstProperty.CanWrite) {
						if (dstProperty.CanWrite && dstProperty.PropertyType.IsAssignableFrom(srcProperty.PropertyType) == true) {
							dstProperty.SetValue(dst, srcProperty.GetValue(src, null), null);
						}
					}
				}
			}
		}
		
		string generateFCRQLog() {
			string template = "INC raised: " + INC + Environment.NewLine;
			template += "Site: " + SiteId + Environment.NewLine;
			template += "CRQ: " + CRQ + Environment.NewLine;
			template += "CRQ contacts:" + Environment.NewLine + Contacts + Environment.NewLine;
//			string[] lines = richTextBox16.Lines;
//			for (int i = 0; i < lines.Length; i++) {
//				if(!string.IsNullOrEmpty(lines[i]))
//					template += lines[i] + Environment.NewLine;
//			}
			template += "FE booked in: " + FEBookedIn_Name + ", " + FEBookedIn_Tel + Environment.NewLine;
			template += "Did FE call the ANOC after CRQ: " + (FECalledANOC ? "Yes" :  "No") + Environment.NewLine;
						
			template += "Work performed by FE on site:" + Environment.NewLine + WorkPerformed + Environment.NewLine;
//			if (!string.IsNullOrEmpty(richTextBox1.Text)) {
//				template += Environment.NewLine;
//				lines = richTextBox1.Lines;
//				for (int i = 0; i < lines.Length; i++) {
//					if(!string.IsNullOrEmpty(lines[i]))
//						template += lines[i] + Environment.NewLine;
//				}
//			}
//			else
//				template += " N/A" + Environment.NewLine;
			
			template += "Troubleshooting done with FE on site to recover affected cells:" + Environment.NewLine + TroubleshootingDone + Environment.NewLine;
//			if (!string.IsNullOrEmpty(richTextBox2.Text)) {
//				template += Environment.NewLine;
//				lines = richTextBox2.Lines;
//				for (int i = 0; i < lines.Length; i++) {
//					if(!string.IsNullOrEmpty(lines[i]))
//						template += lines[i] + Environment.NewLine;
//				}
//			}
//			else
//				template += " N/A" + Environment.NewLine;
			
			template += "Contractor to fix the fault: " + ContractorToFixFault_Name;
			if (ContractorToFixFault_Name != "None provided" || !string.IsNullOrEmpty(ContractorToFixFault_Tel))
				template += ", " + ContractorToFixFault_Tel;
			template += Environment.NewLine;
			template += "Time to fix the fault: " + (ContractorToFixFault_Date == new DateTime(1,1,1) ? "None provided" : ContractorToFixFault_Date.ToString("dd-MM-yyyy H:mm")) + Environment.NewLine;
//			if (ContractorToFixFault_Date != new DateTime(1,1,1))
//				template += dateTimePicker1.Text + Environment.NewLine;
//			else
//				template += "None provided" + Environment.NewLine;
			template += "Observations:" + Environment.NewLine + Observations + Environment.NewLine;
//			if (!string.IsNullOrEmpty(richTextBox3.Text)) {
//				template += Environment.NewLine;
//				lines = richTextBox3.Lines;
//				for (int i = 0; i < lines.Length; i++) {
//					if(!string.IsNullOrEmpty(lines[i]))
//						template += lines[i];
//					if(i != lines.Length -1)
//						template += Environment.NewLine;
//				}
//			}
//			else
//				template += " N/A";
			return template;
		}
		
		void generateFullLog(string html) {
			try { fullLog = appCore.Web.Tools.HtmlRemoval.StripTagsCharArray(html); } catch (Exception) {}
		}
	}
}