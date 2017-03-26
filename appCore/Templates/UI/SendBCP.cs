/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 09-11-2015
 * Time: 13:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using appCore.Templates.Types;
using appCore.UI;

namespace appCore.Templates.UI
{
	/// <summary>
	/// Description of SendBCP.
	/// </summary>
	public partial class SendBCP : Form
	{
		Troubleshoot currentTemplate;
		public string mailBody = string.Empty;
		
		public SendBCP(ref Troubleshoot template)
		{
			currentTemplate = template;
			InitializeComponent();
			// TODO: ReWork SendBCP class
			comboBox1.SelectedIndex = 0;
			comboBox2.SelectedIndex = 0;
			textBox2.Text = currentTemplate.SiteId;
			textBox4.Text = currentTemplate.fullLog;
			button3.Enabled |= !string.IsNullOrEmpty(textBox4.Text);
			
			textBox1.Focus();
			
//			textBox3.Text = "ANOC-";
//			if(template.Contains("Intermittent Issue: YES")) textBox3.Text += "#INTERMITTENT#-";
//			if(template.Contains("COOS: YES")) {
//				string[] strTofind = { "\r\n" };
//				string[] temp = template.Split(strTofind, StringSplitOptions.None);
//				string[] COOS = temp[5].Substring(10,temp[5].Length - 10).Split(' ');
//				textBox3.Text += "COOS: 2G:";
//				if(COOS[1].Length == 1) textBox3.Text += "0";
//				textBox3.Text += COOS[1];
//				textBox3.Text += " 3G:";
//				if(COOS[3].Length == 1) textBox3.Text += "0";
//				textBox3.Text += COOS[3];
//				textBox3.Text += " 4G:";
//				if(COOS[5].Length == 1) textBox3.Text += "0";
//				textBox3.Text += COOS[5];
//			}
//			textBox3.Text += "-";
//			if(radioButton1.Checked) textBox3.Text += "#SL0#-";
//			else if(radioButton2.Checked) textBox3.Text += "#P1#-";
//			if(checkBox2.Checked) textBox3.Text += "#MET#-";
		}
		
		void TextBox1KeyPress(object sender, KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				if (textBox1.Text.Length > 0) {
					string CompTAS = Toolbox.Tools.CompleteINC_CRQ_TAS(textBox1.Text, "TAS");
					if (CompTAS != "error") textBox1.Text = CompTAS;
					else {
						FlexibleMessageBox.Show("Task number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
			}
		}
		
		void TextBox4TextChanged(object sender, EventArgs e)
		{
			button3.Enabled = !string.IsNullOrEmpty(textBox4.Text);
		}
		
		void Button3Click(object sender, EventArgs e)
		{
			Action action = new Action(delegate {
			                           	AMTLargeTextForm enlarge = new AMTLargeTextForm(textBox4.Text,label5.Text,true);
			                           	enlarge.StartPosition = FormStartPosition.CenterParent;
			                           	enlarge.ShowDialog();
			                           	textBox4.Text = enlarge.finaltext;
			                           });
			LoadingPanel load = new LoadingPanel();
			load.ShowAsync(null, action,false,this);
		}
		
		void Button1Click(object sender, EventArgs e)
		{
//			string CompTAS = Toolbox.Tools.CompleteINC_CRQ_TAS(textBox1.Text, "TAS");
//			if (CompTAS != "error") textBox1.Text = CompTAS;
//			else {
//				MessageBox.Show("Task number must only contain digits!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
//				return;
//			}
			string errmsg = "";
			if (string.IsNullOrEmpty(textBox1.Text)) {
				errmsg = "         - Task Number missing\n";
			}
			if (string.IsNullOrEmpty(comboBox1.Text)) {
				errmsg = errmsg + "         - Severity level missing\n"; //           Choose SL0 or P1 site";
			}
			if (string.IsNullOrEmpty(textBox2.Text)) {
				errmsg = errmsg + "         - Site ID missing\n";
			}
			if (string.IsNullOrEmpty(textBox3.Text)) {
				errmsg = errmsg + "         - Title of Fault missing\n";
			}
			if (string.IsNullOrEmpty(textBox4.Text)) {
				errmsg = errmsg + "         - Problem description missing\n";
			}
			if (string.IsNullOrEmpty(comboBox2.Text)) {
				errmsg = errmsg + "         - Email recipient missing\n";
			}
			if (!string.IsNullOrEmpty(errmsg)) {
				FlexibleMessageBox.Show("The following errors were detected\n\n" + errmsg + "\nPlease fill the required fields and try again.", "Data missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			errmsg = string.Empty;
			
//			string[] tempBody = textBox4.Text.Split('\n');
			
			string mailSubject = "BCP - " + textBox2.Text + " / " + textBox1.Text;
			
			mailBody = "<html xmlns:o=\"urn:schemas-microsoft-com:office:office\"xmlns:x=\"urn:schemas-microsoft-com:office:excel\"xmlns=\"http://www.w3.org/TR/REC-html40\"><head><meta http-equiv=Content-Type content=\"text/html; charset=windows-1252\"><style id=\"tableStyle\"><!--table{mso-displayed-decimal-separator:\"\\,\";mso-displayed-thousand-separator:\"\\.\";}.xl1521302{padding-top:1px;padding-right:1px;padding-left:1px;mso-ignore:padding;color:black;font-size:11.0pt;font-weight:400;font-style:normal;text-decoration:none;font-family:Calibri, sans-serif;mso-font-charset:0;mso-number-format:General;text-align:general;vertical-align:bottom;mso-background-source:auto;mso-pattern:auto;white-space:nowrap;}.xl6321302{padding-top:1px;padding-right:1px;padding-left:1px;mso-ignore:padding;color:black;font-size:10.0pt;font-weight:700;font-style:normal;text-decoration:none;font-family:Calibri, sans-serif;mso-font-charset:0;mso-number-format:General;text-align:general;vertical-align:middle;border-top:1.0pt solid black;border-right:1.0pt solid black;border-bottom:.5pt solid black;border-left:1.0pt solid black;mso-background-source:auto;mso-pattern:auto;white-space:normal;}.xl6421302{padding-top:1px;padding-right:1px;padding-left:1px;mso-ignore:padding;color:black;font-size:10.0pt;font-weight:400;font-style:normal;text-decoration:none;font-family:Calibri, sans-serif;mso-font-charset:0;mso-number-format:General;text-align:general;vertical-align:middle;border-top:1.0pt solid black;border-right:1.0pt solid black;border-bottom:.5pt solid black;border-left:none;mso-background-source:auto;mso-pattern:auto;white-space:normal;}.xl6521302{padding-top:1px;padding-right:1px;padding-left:1px;mso-ignore:padding;color:black;font-size:10.0pt;font-weight:700;font-style:normal;text-decoration:none;font-family:Calibri, sans-serif;mso-font-charset:0;mso-number-format:General;text-align:general;vertical-align:middle;border-top:.5pt solid black;border-right:1.0pt solid black;border-bottom:.5pt solid black;border-left:1.0pt solid black;mso-background-source:auto;mso-pattern:auto;white-space:normal;}.xl6621302{padding-top:1px;padding-right:1px;padding-left:1px;mso-ignore:padding;color:black;font-size:10.0pt;font-weight:400;font-style:normal;text-decoration:none;font-family:Calibri, sans-serif;mso-font-charset:0;mso-number-format:General;text-align:general;vertical-align:middle;border-top:.5pt solid black;border-right:1.0pt solid black;border-bottom:.5pt solid black;border-left:none;mso-background-source:auto;mso-pattern:auto;white-space:normal;}.xl6721302{padding-top:1px;padding-right:1px;padding-left:1px;mso-ignore:padding;color:black;font-size:10.0pt;font-weight:400;font-style:normal;text-decoration:none;font-family:Calibri, sans-serif;mso-font-charset:0;mso-number-format:General;text-align:left;vertical-align:middle;border-top:.5pt solid black;border-right:1.0pt solid black;border-bottom:.5pt solid black;border-left:none;mso-background-source:auto;mso-pattern:auto;white-space:normal;}.xl6821302{padding-top:1px;padding-right:1px;padding-left:1px;mso-ignore:padding;color:black;font-size:10.0pt;font-weight:700;font-style:normal;text-decoration:none;font-family:Calibri, sans-serif;mso-font-charset:0;mso-number-format:General;text-align:general;vertical-align:middle;border-top:.5pt solid black;border-right:1.0pt solid black;border-bottom:1.0pt solid black;border-left:1.0pt solid black;mso-background-source:auto;mso-pattern:auto;white-space:normal;}.xl6921302{padding-top:1px;padding-right:1px;padding-left:1px;mso-ignore:padding;color:black;font-size:10.0pt;font-weight:400;font-style:normal;text-decoration:none;font-family:Calibri, sans-serif;mso-font-charset:0;mso-number-format:General;text-align:general;vertical-align:middle;border-top:.5pt solid black;border-right:1.0pt solid black;border-bottom:1.0pt solid black;border-left:none;mso-background-source:auto;mso-pattern:auto;white-space:normal;}body{font-size:10.0pt;font-weight:400;font-style:normal;text-decoration:none;font-family:Calibri, sans-serif;mso-font-charset:0;mso-number-format:General;text-align:general;}--></style></head><body>Hello,<br><br><div id=\"Folha2_21302\" align=left x:publishsource=\"Excel\"><table border=0 cellpadding=0 cellspacing=0 width=410 style='border-collapse: collapse;table-layout:fixed;width:308pt'> <col width=120 style='mso-width-source:userset;mso-width-alt:4388;width:90pt'> <col width=420 style='mso-width-source:userset;mso-width-alt:10605;width:315pt'> <tr style='height:auto'> <td class=xl6321302 width=120 style='height:auto;width:90pt'><span style='mso-fareast-language:EN-US'>Customer Task Number</span></td> <td class=xl6421302 width=420 style='width:315pt'><span style='mso-fareast-language: EN-US'>" + textBox1.Text + "</span></td> </tr> <tr style='height:auto'> <td class=xl6521302 width=120 style='height:auto;border-top:none; width:90pt'><span style='mso-fareast-language:EN-US'>Severity Level</span></td> <td class=xl6621302 width=420 style='border-top:none;width:315pt'><span style='mso-fareast-language:EN-US'>" + comboBox1.Text + "</span></td> </tr> <tr style='height:auto'> <td class=xl6521302 width=120 style='height:auto;border-top:none; width:90pt'><span style='mso-fareast-language:EN-US'>Site ID</span></td> <td class=xl6721302 width=420 style='border-top:none;width:315pt'><span style='mso-fareast-language:EN-US'>" + textBox2.Text + "</span></td> </tr> <tr style='height:auto'> <td class=xl6521302 width=120 style='height:auto;border-top: none;width:90pt'><span lang=EN-US style='mso-ansi-language:EN-US;mso-fareast-language: EN-US'>Title Of Fault (including cell id / site nominal)</span></td> <td class=xl6621302 width=420 style='border-top:none;width:315pt'><span style='mso-fareast-language:EN-US'>" + textBox3.Text + "</span></td> </tr> <tr style='height:auto'> <td class=xl6821302 width=120 style='height:auto;border-top: none;width:90pt'><span lang=EN-US style='mso-ansi-language:EN-US;mso-fareast-language: EN-US'>Problem description including details of what field ops are required to do</span></td> <td class=xl6921302 width=420 style='border-top:none;width:315pt'>" + string.Join("<br>", textBox4.Lines) + "</td> </tr> <![if supportMisalignedColumns]> <tr style='display:none'> <td width=120 style='width:90pt'></td> <td width=420 style='width:315pt'></td> </tr> <![endif]></table></div></body></html>";
			
			Toolbox.Tools.CreateMailItem(comboBox2.Text, "A-NOC-UK1stLineRANSL@internal.vodafone.com", mailSubject, mailBody, true);
			
			// TODO: Store BCP on logs
		}
		
		void RadioButton1CheckedChanged(object sender, EventArgs e)
		{
			
		}
		
		void RadioButton2CheckedChanged(object sender, EventArgs e)
		{
			
		}
		
		void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			
		}
	}
}
